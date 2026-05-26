using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QLXDK.Models;
using QLXDK.Models.Views;
using PagedList;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace QLXDK.Controllers
{
    [Authorize]
    public class VoyageController : Controller
    {
        private qlslContext _db = new qlslContext();
        public ActionResult Index(int? page)
        {
            int pageSize = 10;

            int pageNumber = (page ?? 1);
            ViewBag.CurrentPage = pageNumber;
            var data = (from s in _db.Voyages
                        join d in _db.Salans on s.SalanId equals d.ID
                        join c in _db.DestinationPorts on s.DestinationPortId equals c.ID
                        select new VoyageListVm
                        {
                            ID = s.ID,
                            Name = s.Name,
                            SalanName = d.Name,
                            DestinationPortName = c.PortName,
                            CreatedAt = s.CreatedAt
                        })
           .OrderByDescending(u => u.ID)
           .ToPagedList(pageNumber, pageSize);

            return View(data);
        }

        public ActionResult Details(int id)
        {
            return View();
        }

        public ActionResult Create()
        {
            var ports = _db.DestinationPorts.ToList();
            ViewBag.PortList = new SelectList(ports, "ID", "PortName");

            var salans = _db.Salans.ToList();
            ViewBag.SalanList = new SelectList(salans, "ID", "Name");
            return View();
        }

        public ActionResult CreateSub(int id)
        {
            var des = _db.DestinationPorts.ToList();
            ViewBag.DesPortList = new SelectList(des, "ID", "PortName");

            var loads = _db.PortOfLoadings.ToList();
            ViewBag.LoadList = new SelectList(loads, "ID", "PortName");
            ViewBag.VoyageId = id;
            return View("CreateSub");
        }

        [HttpPost]
        public ActionResult Import()
        {
            string jsonRawData;
            using (var reader = new StreamReader(Request.InputStream))
            {
                jsonRawData = reader.ReadToEnd();
            }

            if (string.IsNullOrEmpty(jsonRawData))
            {
                return Json(new { success = false, message = "Dữ liệu nhập trống!" });
            }

            dynamic payload = JsonConvert.DeserializeObject(jsonRawData);

            int DestinationPortId = (int)payload.DestinationPortId;
            int PortOfLoadingId = (int)payload.PortOfLoadingId;
            int VoyageId = (int)payload.VoyageId;
            List<Dictionary<string, string>> Items = payload.Items.ToObject<List<Dictionary<string, string>>>();
            string[] ContainerList = payload.ContainerList.ToObject<string[]>();

            // Check duplicate SubVoyage
            bool isExist = _db.SubVoyages.Any(x => x.VoyageId == VoyageId
                                    && x.PortOfLoadingId == PortOfLoadingId
                                    && x.DestinationPortId == DestinationPortId);
            if (isExist) {
                return Json(new { success = false, message = "Chuyến này đã được tạo trước đó!" });
            }

            // Check duplicate container
            string[] containersByVoyage = (from s in _db.SubVoyages
                                           join a in _db.VoyageDetails on s.ID equals a.SubVoyageId
                                           select a.ContainerNo).ToArray();
            string[] duplicateConts = containersByVoyage.Intersect(ContainerList).ToArray();
            if (duplicateConts.Any())
            {
                string duplicateList = string.Join(", ", duplicateConts);
                return Json(new { success = false, message = "Lỗi trùng dữ liệu" });
            }

            var newItem = new Models.Entities.SubVoyage
            {
                VoyageId = VoyageId,
                PortOfLoadingId = PortOfLoadingId,
                DestinationPortId = DestinationPortId,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            
           
            int userId = Convert.ToInt32(Session["UserId"]);
            if (Items != null)
            {
                using (var transaction = _db.Database.BeginTransaction())
                {
                    try
                    {
                        _db.SubVoyages.Add(newItem);
                        _db.SaveChanges();

                        var details = Items.Select(item => new QLXDK.Models.Entities.VoyageDetail
                        {
                            SubVoyageId = newItem.ID,
                            ContainerNo = item.ContainsKey("ContainerNo") ? item["ContainerNo"] : null,
                            SizeType = item.ContainsKey("SizeType") ? item["SizeType"] : null,
                            Line = item.ContainsKey("Line") ? item["Line"] : null,
                            BookingBillNo = item.ContainsKey("BookingBillNo") ? item["BookingBillNo"] : null,
                            SealNo = item.ContainsKey("SealNo") ? item["SealNo"] : null,
                            FullEmpty = item.ContainsKey("FullEmpty") ? item["FullEmpty"].Substring(0, 1) : null,
                            Category = item.ContainsKey("Category") ? item["Category"].Substring(0, 1) : null,
                            VesVoyName = item.ContainsKey("VesVoyName") ? item["VesVoyName"] : null,
                            PortOfLoad = item.ContainsKey("PortOfLoad") ? item["PortOfLoad"] : null,
                            PortOfDischarge = item.ContainsKey("PortOfDischarge") ? item["PortOfDischarge"] : null,
                            GrossWeight = float.Parse(item["GrossWeight"].ToString()),
                            VGM = float.Parse(item["VGM"].ToString()),
                            TemperatureC = item.ContainsKey("TemperatureC") ? item["TemperatureC"] : null,
                            IMO = item.ContainsKey("IMO") ? item["IMO"] : null,
                            UN = item.ContainsKey("UN") ? item["UN"] : null,
                            Commodity = item.ContainsKey("Commodity") ? item["Commodity"] : null,
                            Remarks = item.ContainsKey("Remarks") ? item["Remarks"] : null,
                            CreatedBy = userId,
                            CreatedDate = DateTime.Now
                        }).ToList();

                        _db.VoyageDetails.AddRange(details);
                        _db.SaveChanges();

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        Helpers.Logger.LogException(ex);
                        transaction.Rollback();
                        return Json(new { success = false, message = "Import không thành công" });
                    }
                }
            }

            //return Json(new { success = true });
            return Json(new { success = true, message = "Import thành công" });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Models.Views.VoyageVM model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var Name = Request["bargeName"] + "_" + DateTime.Now.ToString("yyyyMMdd");
                    bool isExist = _db.Voyages.Any(x => x.Name == Name);
                    
                    if (isExist)
                    {
                        TempData["Error"] = "Tên chuyến đã tạo rồi";
                        return RedirectToAction("Create");
                    }

                    var newItem = new Models.Entities.Voyage
                    {
                        SalanId = model.SalanId,
                        Name = Name,
                        DestinationPortId = model.DestinationPortId,
                        CreatedAt = DateTime.Now
                    };

                    _db.Voyages.Add(newItem);
                    _db.SaveChanges();

                    TempData["Message"] = "Thêm thành công!";
                    return RedirectToAction("Create");
                    //}
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Lỗi tạo chuyến trùng hoặc xử lý dữ liệu";
                    return RedirectToAction("Create");
                    //ModelState.AddModelError("", "Có lỗi xảy ra: " + ex.Message);
                }
            }

            return View(model);
        }

        public ActionResult Edit(int id)
        {
            try
            {
                var ports = _db.DestinationPorts.ToList();
                ViewBag.PortList = new SelectList(ports, "ID", "PortName");

                var salans = _db.Salans.ToList();
                ViewBag.SalanList = new SelectList(salans, "ID", "Name");

                var model = (from s in _db.Voyages
                            join d in _db.Salans on s.SalanId equals d.ID
                            join c in _db.DestinationPorts on s.DestinationPortId equals c.ID
                            where s.ID == id
                            select new VoyageEditVM
                            {
                                ID = s.ID,
                                Teus = d.Teus,
                                SalanId = d.ID,
                                DestinationPortId = c.ID

                            }).FirstOrDefault();

                return View(model);
            }
            catch (Exception e)
            {
                return View();
            }
        }

        public ActionResult Detail(int id)
        {
            try
            {
                var data = (from s in _db.SubVoyages
                            join a in _db.PortOfLoadings on s.PortOfLoadingId equals a.ID
                            join b in _db.DestinationPorts on s.DestinationPortId equals b.ID
                            where s.VoyageId == id
                            select new SubVoyageDetailVM
                            {
                                ID = s.ID,
                                PortOfLoadingName = a.PortName,
                                DestinationPortName = b.PortName,

                                Amount = _db.VoyageDetails.Count(vd => vd.SubVoyageId == s.ID)
                            }).ToList();
                return View(data);
                //return PartialView("_SubDetail", data);
            }
            catch (Exception e)
            {
                return View();
            }
        }

        public ActionResult SubDetail(int id)
        {
            try
            {
                var data = (from s in _db.VoyageDetails
                            join u in _db.Users on s.ApprovedBy equals u.ID into userGroup
                            from u in userGroup.DefaultIfEmpty()
                            where s.SubVoyageId == id && s.DeletedDate == null
                            select new QLXDK.Models.Views.VoyageDetailRow
                            {
                                Detail = s,
                                ApprovedByUsername = u != null ? u.UserName : "Chưa duyệt"
                            }).ToList();
                ViewBag.SubVoyageId = id;
                return View(data);
                //return PartialView("_SubDetail", data);
            }
            catch (Exception e)
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult Edit(int ID, Models.Views.VoyageEditVM model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(model);

                var voyage = _db.Voyages.SingleOrDefault(p => p.ID == ID);
                if (voyage == null) return HttpNotFound();
                voyage.DestinationPortId = model.DestinationPortId;
                voyage.UpdatedAt = DateTime.Now;

                _db.SaveChanges();
                TempData["Message"] = "Cập nhật thành công!";
                return RedirectToAction("Edit", new { ID = voyage.ID });
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Search(string q, int? page)
        {
            int pageSize = 10;

            int pageNumber = (page ?? 1);
            ViewBag.CurrentPage = pageNumber;
            var data = (from s in _db.Bookings
                        join d in _db.Depots on s.DepotId equals d.ID
                        join c in _db.PortOfLoadings on s.PortOfLoadingId equals c.ID
                        where string.IsNullOrEmpty(q) || s.BookingNo.Contains(q)
                        select new BookingListVm
                        {
                            ID = s.ID,
                            BookingNo = s.BookingNo,
                            Vessel = s.Vessel,
                            DepotName = d.Name,
                            //PortName = c.Name,
                            SailingDate = s.SailingDate,
                            ClosingDate = s.ClosingDate
                        })
           .OrderByDescending(u => u.ID)
           .ToPagedList(pageNumber, pageSize);
            return PartialView("_Search", data);
        }


        public ActionResult Delete(int id)
        {
            try
            {
                // TODO: Add delete logic here
                var item = _db.Voyages.Find(id);
                _db.Voyages.Remove(item);
                _db.SaveChanges();
                TempData["Message"] = "Xoá thành công!";
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }

        }

        public ActionResult DeleteSub(int id)
        {
            try
            {
                // TODO: Add delete logic here
                var item = _db.SubVoyages.Find(id);
                _db.SubVoyages.Remove(item);
                //_db.SaveChanges();
                _db.SaveChanges();

                TempData["Message"] = "Xoá thành công!";
                return Redirect(Request.UrlReferrer.ToString());
                //return RedirectToAction("Detail", new { ID = id });
                //return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }

        }

        [HttpPost]
        //public ActionResult Approve(int SubVoyageId, int[] IDs)
        public ActionResult Approve()
        {
            try {
                string jsonRawData;
                using (var reader = new StreamReader(Request.InputStream))
                {
                    jsonRawData = reader.ReadToEnd();
                }

                if (string.IsNullOrEmpty(jsonRawData))
                {
                    return Json(new { success = false, message = "Dữ liệu InputStream trống!" });
                }

                dynamic payload = JsonConvert.DeserializeObject(jsonRawData);

                int SubVoyageId = (int)payload.SubVoyageId;
                int[] IDs = payload.IDs.ToObject<int[]>();

                var list = _db.VoyageDetails
                    .Where(s => s.SubVoyageId == SubVoyageId &&  IDs.Contains(s.ID))
                    .ToList();
                int userId = Convert.ToInt32(Session["UserId"]);
                foreach (var l in list)
                {
                    l.ApprovedBy = userId;
                    l.ApprovedDate = DateTime.Now;
                }

                _db.SaveChanges();

                var data = (from s in _db.VoyageDetails
                            join u in _db.Users on s.ApprovedBy equals u.ID into userGroup
                            from u in userGroup.DefaultIfEmpty()
                            where s.SubVoyageId == SubVoyageId && s.DeletedDate == null
                            select new QLXDK.Models.Views.VoyageDetailRow
                            {
                                Detail = s,
                                ApprovedByUsername = u != null ? u.UserName : "Chưa duyệt"
                            }).ToList();
                ViewBag.SubVoyageId = SubVoyageId;

                TempData["Message"] = "Duyệt thành công!";
                return PartialView("_SubDetail", data);
            }
            catch(Exception e)
            {
                return Json(new { success = false, message = "Lỗi xử lý dữ liệu" });
            }

        }


        [HttpPost]
        public ActionResult Delete(int SubVoyageId, int[] IDs)
        {
            try
            {
                var list = _db.VoyageDetails
                    .Where(s => s.SubVoyageId == SubVoyageId && IDs.Contains(s.ID))
                    .ToList();
                int userId = Convert.ToInt32(Session["UserId"]);

                foreach (var l in list)
                {
                    l.DeletedBy = userId;
                    l.DeletedDate = DateTime.Now;
                }

                _db.SaveChanges();

                var data = (from s in _db.VoyageDetails
                            join u in _db.Users on s.ApprovedBy equals u.ID into userGroup
                            from u in userGroup.DefaultIfEmpty()
                            where s.SubVoyageId == SubVoyageId && s.DeletedDate == null
                            select new QLXDK.Models.Views.VoyageDetailRow
                            {
                                Detail = s,
                                ApprovedByUsername = u != null ? u.UserName : "Chưa duyệt"
                            }).ToList();

                ViewBag.SubVoyageId = SubVoyageId;

                TempData["Message"] = "Xoá thành công!";
                return PartialView("_SubDetail", data);
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = "Lỗi xử lý dữ liệu" });
            }

        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QLXDK.Models;
using QLXDK.Models.Views;
using PagedList;

namespace QLXDK.Controllers
{
    [Authorize]
    public class BookingController : Controller
    {
        private qlslContext _db = new qlslContext();
        public ActionResult Index(int? page)
        {
            int pageSize = 10;

            int pageNumber = (page ?? 1);
            ViewBag.CurrentPage = pageNumber;
            var data = (from s in _db.Bookings
                        join d in _db.Depots on s.DepotId equals d.ID
                        join c in _db.PortOfLoadings on s.PortOfLoadingId equals c.ID
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

            return View(data);
        }

        public ActionResult Details(int id)
        {
            return View();
        }

        public ActionResult Create()
        {
            var ports = _db.PortOfLoadings.ToList();
            ViewBag.PortList = new SelectList(ports, "ID", "Name");

            // Fetch Depots
            var depots = _db.Depots.ToList();
            ViewBag.DepotList = new SelectList(depots, "ID", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Models.Views.BookingVM model)
        {
            //if (ModelState.IsValid)
            //{
                try
                {
                    var newItem = new Models.Entities.Booking
                    {
                        BookingNo = model.BookingNo,
                        DepotId = model.DepotId,
                        Vessel = model.Vessel,
                        PortOfLoadingId = model.PortOfLoadingId,
                        SailingDate = model.SailingDate,
                        ClosingDate = model.ClosingDate, 
                        SlClosingTime = model.SlClosingTime,
                        VgmClosingDate = model.VgmClosingDate,
                        VgmClosingTime = model.VgmClosingTime,
                        Quantity = model.Quantity,
                        PickupAt = model.PickupAt,
                        ReturnAt = model.ReturnAt,
                        Remark = model.Remark
                    };

                    _db.Bookings.Add(newItem);
                    _db.SaveChanges();

                    TempData["Message"] = "Thêm thành công!";
                    return RedirectToAction("Create");
                    //}
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Có lỗi xảy ra: " + ex.Message);
                }
            //}

            return View(model);
        }

        public ActionResult Edit(int id)
        {
            try
            {

                var booking = _db.Bookings.Find(id);
                var depots = _db.Depots
                   .Select(d => new { d.ID, d.Name })
                   .ToList();

                var ports = _db.PortOfLoadings
                   .Select(d => new { d.ID })
                   .ToList();

                ViewBag.DepotList = new SelectList(depots, "ID", "Name", booking.DepotId);
                ViewBag.PortList = new SelectList(ports, "ID", "Name", booking.PortOfLoadingId);

                var model = new Models.Views.BookingVM
                {
                    ID = booking.ID,
                    BookingNo = booking.BookingNo,
                    Vessel = booking.Vessel,
                    PortOfLoadingId = booking.PortOfLoadingId,
                    SailingDate = booking.SailingDate,
                    ClosingDate = booking.ClosingDate,
                    SlClosingTime = booking.SlClosingTime,
                    VgmClosingDate = booking.VgmClosingDate,
                    VgmClosingTime = booking.VgmClosingTime,
                    Quantity = booking.Quantity,
                    PickupAt = booking.PickupAt,
                    ReturnAt = booking.ReturnAt,
                    Remark = booking.Remark
                };

                return View(model);
            }
            catch(Exception e)
            {
                return View();
            }
        }

        public ActionResult Detail(int id)
        {
            try
            {
                var data = (from s in _db.Bookings
                            join d in _db.Depots on s.DepotId equals d.ID
                            join c in _db.PortOfLoadings on s.PortOfLoadingId equals c.ID
                            where s.ID == id
                            select new BookingListVm
                            {
                                ID = s.ID,
                                BookingNo = s.BookingNo,
                                Vessel = s.Vessel,
                                DepotName = d.Name,
                                //PortName = c.Name,
                                SailingDate = s.SailingDate,
                                ClosingDate = s.ClosingDate,
                                SlClosingTime = s.SlClosingTime,
                                VgmClosingDate = s.VgmClosingDate,
                                VgmClosingTime = s.VgmClosingTime,
                                Quantity = s.Quantity,
                                ReturnAt = s.ReturnAt,
                                PickupAt = s.PickupAt,
                                Remark = s.Remark
                            }).FirstOrDefault();

                return PartialView("_Detail", data);
            }
            catch (Exception e)
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult Edit(int ID, Models.Views.BookingVM model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(model);

                var booking = _db.Bookings.SingleOrDefault(p => p.ID == ID);
                if (booking == null) return HttpNotFound();

                booking.BookingNo = model.BookingNo;
                booking.Vessel = model.Vessel;
                booking.PortOfLoadingId = model.PortOfLoadingId;
                booking.SailingDate = model.SailingDate;
                booking.ClosingDate = model.ClosingDate;
                booking.SlClosingTime = model.SlClosingTime;
                booking.VgmClosingDate = model.VgmClosingDate;
                booking.VgmClosingTime = model.VgmClosingTime;
                booking.Quantity = model.Quantity;
                booking.PickupAt = model.PickupAt;
                booking.ReturnAt = model.ReturnAt;
                booking.Remark = model.Remark;

                _db.SaveChanges();
                TempData["Message"] = "Cập nhật thành công!";
                return RedirectToAction("Edit", new { ID = booking.ID });
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
                var item = _db.Bookings.Find(id);
                _db.Bookings.Remove(item);
                _db.SaveChanges();
                TempData["Message"] = "Xoá thành công!";
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }

        }
    }
}

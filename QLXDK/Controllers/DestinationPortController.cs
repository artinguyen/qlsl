using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QLXDK.Models;
using PagedList;

namespace QLXDK.Controllers
{
    [Authorize]
    public class DestinationPortController : Controller
    {
        private qlslContext _db = new qlslContext();
        public ActionResult Index(int? page)
        {
            int pageSize = 10;

            int pageNumber = (page ?? 1);
            ViewBag.CurrentPage = pageNumber;
            var data = _db.DestinationPorts
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

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Models.Views.DestinationPortVM model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var newItem = new Models.Entities.DestinationPort
                    {
                        PortCode = model.PortCode,
                        PortName = model.PortName,
                        Status = model.Status,
                        Description = model.Description,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };

                    _db.DestinationPorts.Add(newItem);
                    _db.SaveChanges();

                    TempData["Message"] = "Thêm thành công!";
                    return RedirectToAction("Create");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Có lỗi xảy ra: " + ex.Message);
                }
            }

            return View(model);
        }

        public ActionResult Edit(int id)
        {
            try
            {

                var item = _db.DestinationPorts.Find(id);

                var model = new Models.Views.DestinationPortVM
                {
                    ID = item.ID,
                    PortCode = item.PortCode,
                    PortName = item.PortName,
                    Status = item.Status,
                    Description = item.Description

                };

                return View(model);
            }
            catch
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult Edit(int ID, Models.Views.DestinationPortVM model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(model);

                var item = _db.DestinationPorts.SingleOrDefault(p => p.ID == ID);
                if (item == null) return HttpNotFound();

                item.PortCode = model.PortCode;
                item.PortName = model.PortName;
                item.Status = model.Status;
                item.Description = model.Description;
                item.UpdatedAt = DateTime.Now;

                _db.SaveChanges();
                TempData["Message"] = "Cập nhật thành công!";
                return RedirectToAction("Edit", new { ID = item.ID });
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Delete(int id)
        {
            try
            {
                var item = _db.DestinationPorts.Find(id);
                _db.DestinationPorts.Remove(item);
                _db.SaveChanges();
                TempData["Message"] = "Xoá thành công!";
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }

        }

        public ActionResult Excel()
        {
            return View();
        }
    }
}

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
    public class DepotController : Controller
    {
        private qlslContext _db = new qlslContext();
        public ActionResult Index(int? page)
        {
            int pageSize = 10;

            int pageNumber = (page ?? 1);
            ViewBag.CurrentPage = pageNumber;

            var data = _db.Depots
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
        public ActionResult Create(Models.Views.DepotVM model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var newItem = new Models.Entities.Depot
                    {
                        Name = model.Name,
                        Address = model.Address,
                    };

                    _db.Depots.Add(newItem);
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
                var depot = _db.Depots.Find(id);

                var model = new Models.Views.DepotVM
                {
                    ID = depot.ID,
                    Name = depot.Name,
                    Address = depot.Address
                };

                return View(model);
            }
            catch
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult Edit(int ID, Models.Views.DepotVM model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(model);

                var depot = _db.Depots.SingleOrDefault(p => p.ID == ID);
                if (depot == null) return HttpNotFound();

                depot.Name = model.Name;
                depot.Address = model.Address;

                _db.SaveChanges();
                TempData["Message"] = "Cập nhật thành công!";
                return RedirectToAction("Edit", new { ID = depot.ID });
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
                // TODO: Add delete logic here
                var item = _db.Depots.Find(id);
                _db.Depots.Remove(item);
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

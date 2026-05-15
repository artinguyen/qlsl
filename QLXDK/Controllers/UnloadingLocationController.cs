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
    public class UnloadingLocationController : Controller
    {
        private qlslContext _db = new qlslContext();
        // GET: Customer
        public ActionResult Index(int? page)
        {
            int pageSize = 10;

            int pageNumber = (page ?? 1);
            ViewBag.CurrentPage = pageNumber;
            var data = _db.UnloadingLocations
                 .OrderByDescending(u => u.ID)
                 .ToPagedList(pageNumber, pageSize);

            return View(data);
        }

        // GET: Customer/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Customer/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Customer/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Models.Views.UnloadingLocationVM model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var newItem = new Models.Entities.UnloadingLocation
                    {
                        Name = model.Name,
                        Address = model.Address,
                    };

                    _db.UnloadingLocations.Add(newItem);
                    _db.SaveChanges();

                    TempData["Message"] = "Thêm thành công!";
                    return RedirectToAction("Create");
                    //}
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Có lỗi xảy ra: " + ex.Message);
                }
            }

            return View(model);
        }

        // GET: Customer/Edit/5
        public ActionResult Edit(int id)
        {
            try
            {
                var item = _db.UnloadingLocations.Find(id);

                var model = new Models.Views.UnloadingLocationVM
                {
                    ID = item.ID,
                    Name = item.Name,
                    Address = item.Address
                };

                return View(model);
            }
            catch
            {
                return View();
            }
        }

        // POST: Customer/Edit/5
        [HttpPost]
        public ActionResult Edit(int ID, Models.Views.UnloadingLocationVM model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(model);

                var item = _db.UnloadingLocations.SingleOrDefault(p => p.ID == ID);
                if (item == null) return HttpNotFound();

                item.Name = model.Name;
                item.Address = model.Address;

                _db.SaveChanges();
                TempData["Message"] = "Cập nhật thành công!";
                return RedirectToAction("Edit", new { ID = item.ID });
            }
            catch
            {
                return View();
            }
        }

        // POST: Customer/Delete/5
        //[HttpPost]
        public ActionResult Delete(int id)
        {
            try
            {
                // TODO: Add delete logic here
                var item = _db.UnloadingLocations.Find(id);
                _db.UnloadingLocations.Remove(item);
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

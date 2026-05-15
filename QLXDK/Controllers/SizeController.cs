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
    public class SizeController : Controller
    {
        private qlslContext _db = new qlslContext();
        public ActionResult Index(int? page)
        {
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            ViewBag.CurrentPage = pageNumber;

            var data = _db.Sizes
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
        public ActionResult Create(Models.Entities.Size model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var newItem = new Models.Entities.Size
                    {
                        Name = model.Name,
                    };

                    _db.Sizes.Add(newItem);
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
                var size = _db.Sizes.Find(id);

                var model = new Models.Entities.Size { 
                    ID = size.ID,
                    Name = size.Name
                };

                return View(model);
            }
            catch
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult Edit(int ID, Models.Entities.Size model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(model);

                var size = _db.Sizes.SingleOrDefault(p => p.ID == ID);
                if (size == null) return HttpNotFound();

                size.Name = model.Name;
                

                _db.SaveChanges();
                TempData["Message"] = "Cập nhật thành công!";
                return RedirectToAction("Edit", new { ID = size.ID });
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
                var item = _db.Sizes.Find(id);
                _db.Sizes.Remove(item);
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

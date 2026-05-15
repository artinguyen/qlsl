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
    public class SalanController : Controller
    {
        private qlslContext _db = new qlslContext();
        public ActionResult Index(int? page)
        {
            int pageSize = 10;

            int pageNumber = (page ?? 1);
            ViewBag.CurrentPage = pageNumber;

            var data = _db.Salans
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
        public ActionResult Create(Models.Views.SalanVM model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    bool isExist = _db.Salans.Any(x => x.Name == model.Name);
                    if (isExist)
                    {
                        TempData["Error"] = "Tên sà lan đã tạo rồi";
                        return RedirectToAction("Create");
                    }

                    var newItem = new Models.Entities.Salan
                    {
                        Name = model.Name,
                        Teus = model.Teus
                    };

                    _db.Salans.Add(newItem);
                    _db.SaveChanges();

                    TempData["Message"] = "Thêm thành công!";
                    return RedirectToAction("Create");
                }
                catch (Exception ex)
                {
                    TempData["Message"] = "Lỗi trùng tên sà lan hoặc xử lý dữ liệu";
                    ModelState.AddModelError("", "Có lỗi xảy ra: " + ex.Message);
                }
            }

            return View(model);
        }

        public ActionResult Edit(int id)
        {
            try
            {
                var item = _db.Salans.Find(id);

                var model = new Models.Views.SalanVM
                {
                    ID = item.ID,
                    Name = item.Name,
                    Teus = item.Teus
                };

                return View(model);
            }
            catch
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult Edit(int ID, Models.Views.SalanVM model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(model);

                var item = _db.Salans.SingleOrDefault(p => p.ID == ID);
                if (item == null) return HttpNotFound();

                item.Name = model.Name;
                item.Teus = model.Teus;

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
                // TODO: Add delete logic here
                var item = _db.Salans.Find(id);
                _db.Salans.Remove(item);
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

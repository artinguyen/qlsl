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
    public class CustomerController : Controller
    {
        private qlslContext _db = new qlslContext();
        public ActionResult Index(int? page)
        {
            int pageSize = 10;

            int pageNumber = (page ?? 1);
            ViewBag.CurrentPage = pageNumber;
            var data = _db.Customers
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
        public ActionResult Create(Models.Views.CustomerVM model)
        {
            if (ModelState.IsValid)
            {
                try
                {

                    var newUser = new Models.Entities.Customer
                    {
                        FullName = model.FullName,
                        Address = model.Address,
                    };

                    _db.Customers.Add(newUser);
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
                var customer = _db.Customers.Find(id);

                var model = new Models.Views.CustomerVM
                {
                    ID = customer.ID,
                    FullName = customer.FullName,
                    Address = customer.Address
                };

                return View(model);
            }
            catch
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult Edit(int ID, Models.Views.CustomerVM model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(model);

                var customer = _db.Customers.SingleOrDefault(p => p.ID == ID);
                if (customer == null) return HttpNotFound();

                customer.FullName = model.FullName;
                customer.Address = model.Address;

                _db.SaveChanges();
                TempData["Message"] = "Cập nhật thành công!";
                return RedirectToAction("Edit", new { ID = customer.ID });
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
                var item = _db.Customers.Find(id);
                _db.Customers.Remove(item);
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

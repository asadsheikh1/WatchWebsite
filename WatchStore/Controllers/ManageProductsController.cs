using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WatchStore.Filters.AdminSessionFilter;
using WatchStore.Models;

namespace WatchStore.Controllers
{
    [ClassAdminSessionFilter]
    public class ManageProductsController : Controller
    {
        private WatchStoreEntities db = new WatchStoreEntities();

        // GET: ManageProducts
        public ActionResult Index()
        {
            var products = db.products.Include(p => p.categories);
            return View(products.ToList());
        }

        // GET: ManageProducts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            products products = db.products.Find(id);
            if (products == null)
            {
                return HttpNotFound();
            }
            return View(products);
        }

        // GET: ManageProducts/Create
        public ActionResult Create()
        {
            ViewBag.fk_categoryId = new SelectList(db.categories, "categoryId", "categoryName");
            return View(new products());
        }

        // POST: ManageProducts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "productId,fk_categoryId,productName,productPrice,productQty,productImageFile,productDesc")] products products)
        {
            if (ModelState.IsValid)
            {
                HttpPostedFileBase file = Request.Files["productImageFile"];
                if (products.productImageFile != null)
                {
                    string fileName = Path.GetFileNameWithoutExtension(file.FileName);
                    string extension = Path.GetExtension(file.FileName);
                    fileName = fileName + DateTime.Now.ToString("yymmssff") + extension;
                    products.productImage = "~/Content/images/uploads/" + fileName;
                    string path = Path.Combine(Server.MapPath("~/Content/images/uploads/"), fileName);  
                    file.SaveAs(path);
                    db.products.Add(products);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewData["invalidMessage"]= "Please select the message";
                    ViewBag.fk_categoryId = new SelectList(db.categories, "categoryId", "categoryName", products.fk_categoryId);
                    return View(products);
                }
           
            }

            ViewBag.fk_categoryId = new SelectList(db.categories, "categoryId", "categoryName", products.fk_categoryId);
            return View(products);
        }

        // GET: ManageProducts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            products products = db.products.Find(id);
            if (products == null)
            {
                return HttpNotFound();
            }
            ViewBag.fk_categoryId = new SelectList(db.categories, "categoryId", "categoryName", products.fk_categoryId);
            return View(products);
        }

        // POST: ManageProducts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "productId,fk_categoryId,productName,productPrice,productQty,productImage,productImageFile,productDesc")] products products)
        {
            if (products.productImageFile != null)
            {
                HttpPostedFileBase file = Request.Files["productImageFile"];
                string fileName = Path.GetFileNameWithoutExtension(file.FileName);
                string extension = Path.GetExtension(file.FileName);
                fileName = fileName + DateTime.Now.ToString("yymmssff") + extension;
                products.productImage = "~/Content/images/uploads/" + fileName;
                string path = Path.Combine(Server.MapPath("~/Content/images/uploads/"), fileName);
                file.SaveAs(path);

            }
            db.Entry(products).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: ManageProducts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            products products = db.products.Find(id);
            if (products == null)
            {
                return HttpNotFound();
            }
            return View(products);
        }

        // POST: ManageProducts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            products products = db.products.Find(id);
            db.products.Remove(products);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

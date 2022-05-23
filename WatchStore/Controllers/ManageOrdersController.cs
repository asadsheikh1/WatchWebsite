using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WatchStore.Filters.AdminSessionFilter;
using WatchStore.Models;

namespace WatchStore.Controllers
{
    [ClassAdminSessionFilter]
    public class ManageOrdersController : Controller
    {
        private WatchStoreEntities db = new WatchStoreEntities();

        // GET: ManageOrders
        public ActionResult Index()
        {
            var orders = db.orders.Include(o => o.customers);
            return View(orders.ToList());
        }

        // GET: ManageOrders/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var orders = db.orders.Find(id);
            var order_details = db.order_details.Where(x => x.fk_orderId == orders.orderId).ToList();
            return View(order_details);
        }

        // GET: ManageOrders/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            orders orders = db.orders.Find(id);
            if (orders == null)
            {
                return HttpNotFound();
            }
            ViewBag.fk_customerId = new SelectList(db.customers, "customerId", "customerName", orders.fk_customerId);
            return View(orders);
        }

        // POST: ManageOrders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, [Bind(Include = "fullName,phoneNumber,addressDetail,orderStatus")] orders order)
        {
            var _contextOrder = db.orders.Find(id);
            _contextOrder.fullName = order.fullName;
            _contextOrder.phoneNumber = order.phoneNumber;
            _contextOrder.addressDetail = order.addressDetail;
            _contextOrder.orderStatus = order.orderStatus;
            if (order.orderStatus == 3)
            {
                var result = (from orders in db.orders
                              join order_detail
                              in db.order_details on orders.orderId equals
                              order_detail.fk_orderId
                              where orders.orderId == id
                              select new
                              {
                                  productId = order_detail.fk_productId,
                                  productQty = order_detail.qty
                              }).ToList();
                foreach (var item in result)
                {
                    var _contextProductQty = db.products.Where(x => x.productId == item.productId).FirstOrDefault();
                    _contextProductQty.productQty = _contextProductQty.productQty + item.productQty;
                    db.SaveChanges();
                }
            }
            db.Entry(_contextOrder).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        // GET: ManageOrders/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            orders orders = db.orders.Find(id);
            if (orders == null)
            {
                return HttpNotFound();
            }
            return View(orders);
        }

        // POST: ManageOrders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            orders orders = db.orders.Find(id);
            db.orders.Remove(orders);
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

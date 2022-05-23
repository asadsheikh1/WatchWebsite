using WatchStore.Filters.CustomerSessionFilter;
using WatchStore.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WatchStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly WatchStoreEntities db = new WatchStoreEntities();
        public ActionResult Index(int? id)
        {
            if (Session["customerId"] != null)
            {
                var result = CustomersCartItems();
                int totalCartItems = result.Count;
                Session["totalCartItems"] = totalCartItems.ToString();
            }

            ViewBag.Categories = db.categories.OrderBy(x => x.categoryId).ToList();
            if (id != null)
            {
                var _categoryContext = db.categories.Find(id);
                if (_categoryContext != null)
                {
                    ViewBag.categoryName = _categoryContext.categoryName;
                    var _contextProducts = db.products.Where(x => x.fk_categoryId == id);
                    return View(_contextProducts.ToList());
                }
            }
            else
            {
                var _contextProducts = db.products.ToList();
                return View(_contextProducts);
            }
            return View();
        }

        public ActionResult Products()
        {
            return View(db.products.ToList());
        }

        [ClassCustomerSessionFilter]
        public ActionResult Cart()
        {
            var result = CustomersCartItems();
            int totalCartItems = result.Count;
            Session["totalCartItems"] = totalCartItems.ToString();
            return View(result);
        }
        [HttpPost]
        [ClassCustomerSessionFilter]
        public ActionResult AddToCart(int productId, int productQty)
        {
            var customerId = Int32.Parse(Session["customerId"].ToString());
            var _contextProduct = db.products.Find(productId);
            if (_contextProduct.productQty < productQty)
            {
                TempData["itemOutOffStock"] = "Hi, item is out of stock";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                var _contextCart = db.cart.Where(x => x.fk_productId == productId && x.fk_customerId ==customerId).FirstOrDefault();
                if (_contextCart != null)
                {
                    _contextCart.totalPrice = _contextCart.totalPrice + (_contextProduct.productPrice * productQty);
                    _contextCart.qty = _contextCart.qty + productQty;
                    db.Entry(_contextCart).State = EntityState.Modified;
                    _contextProduct.productQty = _contextProduct.productQty - productQty;
                    db.Entry(_contextCart).State = EntityState.Modified;
                    db.Entry(_contextCart).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    var data = new cart();
                    data.fk_customerId = customerId;
                    data.fk_productId = productId;
                    data.qty = productQty;
                    data.totalPrice = _contextProduct.productPrice * productQty;
                    db.cart.Add(data);
                    _contextProduct.productQty = _contextProduct.productQty - productQty;
                    db.Entry(_contextProduct).State = EntityState.Modified;
                    db.SaveChanges();
                }
                TempData["itemAddedIntoCart"] = "Product is added into cart";
                return RedirectToAction("Index", "Home");
            }
        }
        [ClassCustomerSessionFilter]

        public ActionResult RemoveFromCart(int id)
        {
            var _contextCart = db.cart.Find(id);
            var _contextProduct = db.products.Find(_contextCart.fk_productId);
            _contextProduct.productQty = _contextProduct.productQty + _contextCart.qty;
            db.Entry(_contextProduct).State = EntityState.Modified;
            db.cart.Remove(_contextCart);
            db.SaveChanges();
            return RedirectToAction("Cart", "Home");
        }

        public ActionResult DecreaseQuantity(int id)
        {
            var _contextCart = db.cart.Find(id);
            var _contextProduct = db.products.Find(_contextCart.fk_productId);
            if (_contextCart.qty < 2)
            {
                TempData["cartProductQntyCount"] = "Hi, quantity can not be decrease, minimum should be 1 for product";
            }
            else
            {
                _contextProduct.productQty = _contextProduct.productQty + 1;
                _contextCart.qty = _contextCart.qty - 1;
                _contextCart.totalPrice = _contextCart.totalPrice - _contextProduct.productPrice;
                db.Entry(_contextProduct).State = EntityState.Modified;
                db.Entry(_contextCart).State = EntityState.Modified;
                db.SaveChanges();
            }

            return RedirectToAction("Cart", "Home");
        }
        public ActionResult IncreaseQuantity(int id)
        {
            var _contextCart = db.cart.Find(id);
            var _contextProduct = db.products.Find(_contextCart.fk_productId);
            if (_contextProduct.productQty > 0)
            {
                _contextProduct.productQty = _contextProduct.productQty - 1;
                _contextCart.qty = _contextCart.qty + 1;
                _contextCart.totalPrice = _contextCart.totalPrice + _contextProduct.productPrice;
                db.Entry(_contextProduct).State = EntityState.Modified;

                db.SaveChanges();
            }
            else
            {
                TempData["itemOutOffStock"] = "Hi, item is out of stock";
            }

            return RedirectToAction("Cart", "Home");
        }
        [HttpGet]
        [ClassCustomerSessionFilter]
        public ActionResult Checkout()
        {
            var result = CustomersCartItems();
            ViewBag.checkOutItems = result;
            return View(new orders());
        }
        [ClassCustomerSessionFilter]
        public ActionResult Process(orders order)
        {
            var customerId = Int32.Parse(Session["customerId"].ToString());
            double subTotals = db.cart.Where(c => c.fk_customerId == customerId)
            .Select(i => i.totalPrice).Sum();
            order.fk_customerId = customerId;
            order.totalPrice = subTotals;
            order.orderDate = DateTime.Now.ToString();
            order.orderStatus = 0;
            db.orders.Add(order);
            db.SaveChanges();
            var _contextCart = db.cart.Where(x => x.fk_customerId == customerId).ToList();
            foreach (var items in _contextCart)
            {
                var itemPrice = db.products.Find(items.fk_productId);
                var orderDetail = new order_details();
                orderDetail.fk_orderId = order.orderId;
                orderDetail.fk_productId = items.fk_productId;
                orderDetail.qty = items.qty;
                orderDetail.price = itemPrice.productPrice;
                db.order_details.Add(orderDetail);
                db.SaveChanges();
                db.cart.Remove(items);
                db.SaveChanges();
            }
            TempData["checkoutConfirm"] = "Your order has been confirmed successfully";
            return RedirectToAction("Index", "Home");
        }


        [HttpPost]
        [ClassCustomerSessionFilter]
        public ActionResult Checkout(orders order)
        {
            return Process(order);
        }

        public List<CustomerCartItems> CustomersCartItems()
        {
            var customerId = Int32.Parse(Session["customerId"].ToString());
            var result = (from products in db.products
                          join cart in db.cart
                          on products.productId equals cart.fk_productId
                          join customer in db.customers on cart.fk_customerId equals customer.customerId
                          where customer.customerId == customerId
                          select new CustomerCartItems
                          {
                              cartId = cart.cartId,
                              customerId = customer.customerId,
                              productId = products.productId,
                              productName = products.productName,
                              productPrice = products.productPrice,
                              cartQty = cart.qty,
                              productTotalPrice = cart.totalPrice

                          }).ToList();
            return result;
        }
        [ClassCustomerSessionFilter]
        public ActionResult MyOrders()
        {
            var customerId = Int32.Parse(Session["customerId"].ToString());
            var result = db.orders.Where(x => x.fk_customerId == customerId).OrderByDescending(x => x.orderId).ToList();
            return View(result);
        }
        [ClassCustomerSessionFilter]
        public ActionResult CancelOrder(int id)
        {
            var _contextOrder = db.orders.Find(id);
            _contextOrder.orderStatus = 3;
            db.Entry(_contextOrder).State = EntityState.Modified;
            db.SaveChanges();
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
            TempData["orderCancelled"] = "Your order has been cancelled";
            return RedirectToAction("MyOrders");
        }

        [ClassCustomerSessionFilter]
        // GET: Home/OrderDetails/5
        public ActionResult OrderDetails(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            var orders = db.orders.Find(id);
            var order_details = db.order_details.Where(x => x.fk_orderId == orders.orderId).ToList();
            return View(order_details);
        }

        [HttpPost]
        public ActionResult Search(string searchQuery)
        {
            var result = db.products.Where(x => x.productName.Contains(searchQuery)).ToList();
            ViewBag.searchQuery = searchQuery;
            return View(result);
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
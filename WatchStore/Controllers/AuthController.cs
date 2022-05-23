using WatchStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace WatchStore.Controllers
{
    public class AuthController : Controller
    {
        private readonly WatchStoreEntities _context =new WatchStoreEntities();
        
        [HttpGet]
        // GET: Auth/Login
        public ActionResult Login()
        {
            if (Session["customerId"] != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View(new customers());
        }

        // POST: Auth/Login
        [HttpPost]
        public ActionResult Login(customers user)
        {
            var result = _context.customers.Where(e => e.customerEmail == user.customerEmail && e.customerPassword == user.customerPassword).FirstOrDefault();
            if (result != null)
            {
                Session["customerFullName"] = result.customerName;
                Session["customerId"] = result.customerId;
                Session["role"] = result.role;
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewData["invalidMessage"] = "Email or password is wrong";
                return View(new customers());
            }

        }

        [HttpGet]
        // GET: Auth/Register
        public ActionResult Register()
        {
            return View(new WatchStore.Models.customers());
        }

        [HttpPost]
        // GET: Auth/Register
        public ActionResult Register(customers customer)
        {
            var result = _context.customers.Where(x => x.customerEmail == customer.customerEmail).FirstOrDefault();
            if (result != null)
            {
                ViewData["errorMessage"] = "Email is already registered";
            }
            else
            {
                customer.role = "customer";
                _context.customers.Add(customer);
                _context.SaveChanges();
                TempData["successMessage"] = "Your account is created please login";
                return RedirectToAction("Login", "Auth");
            }
            return View(new customers());
        }


        [HttpGet]
        // GET: Auth/SignIn
        //for admin
        public ActionResult SignIn()
        {
            if (Session["adminId"] != null)
            {
                return RedirectToAction("Index", "ManageCategories");
            }
            return View(new admin());
        }

        // POST: Auth/SignIn
        //for admin
        [HttpPost]
        public ActionResult SignIn(admin admin)
        {
            var result = _context.admin.Where(e => e.username == admin.username && e.password == admin.password).FirstOrDefault();
            if (result != null)
            {
                Session["adminFullName"] = result.fullName;
                Session["adminId"] = result.adminId.ToString();
                Session["role"] = result.role;
                return RedirectToAction("Index", "ManageCategories");
            }
            else
            {
                ViewData["invalidMessage"] = "Username or password is wrong";
                return View(new admin());
            }

        }


        //for admin
        public ActionResult AdminLogout()
        {
            Session.RemoveAll();
            return RedirectToAction("SignIn", "Auth");

        }
        //for customer
        public ActionResult Logout()
        {
            Session.RemoveAll();
            return RedirectToAction("Index", "Home");
        }
    }
}
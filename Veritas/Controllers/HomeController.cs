using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Veritas.Models;

namespace Veritas.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            string username = (string)System.Web.HttpContext.Current.Session["username"];
            if (username != null)
            {
                return View();
            }

            return RedirectToAction("Login", "Home");
        }

        //created by me chinedum
        [HttpGet]
        public ActionResult UserProfile()
        {
            string username = (string)System.Web.HttpContext.Current.Session["username"];
            if (username == null)
            {
                return View();
            }

            return RedirectToAction("Login", "Home");
        }

        public ActionResult Login()
        {
            

            //return RedirectToAction("Home", "Login");

            return View();
        }


        public ActionResult auth(string email, string password)
        {
            var user = (from u in db.Users where u.EMAIL == email && u.PASSWORD == password select u).FirstOrDefault();

            if (user != null)
            {
                System.Web.HttpContext.Current.Session.Add("username", user.USERNAME);
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("Login", "Home");
        }
    }
}

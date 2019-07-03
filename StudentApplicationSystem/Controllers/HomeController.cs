using StudentApplicationSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StudentApplicationSystem.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string email, string password)
        {
            User obj;
            if (ModelState.IsValid)
            {
                using (StudentApplicationSystemDBEntities1 db = new StudentApplicationSystemDBEntities1())
                {
                    obj = db.Users.Where(a => a.email.Equals(email) && a.password.Equals(password)).FirstOrDefault();
                    if (obj != null)
                    {
                        Session["UserID"] = obj.userId.ToString();
                        Session["UserName"] = obj.email.ToString();
                        return RedirectToAction("Index");
                    }
                }
            }
            else
            {
                return View();
            }
                
            return View(obj);
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
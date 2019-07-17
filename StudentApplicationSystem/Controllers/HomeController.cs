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
        private StudentApplicationSystemEntities db = new StudentApplicationSystemEntities();
        public ActionResult Index()
        {
            IList<New> news = db.News.Where(a => a.featured == 1).ToList();
            ViewBag.News = news;
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login([Bind(Include = "email,password")] User user)
        {
            System.Diagnostics.Debug.WriteLine("Debugging Login");
            
            if (ModelState.IsValid)
            {
                using (StudentApplicationSystemEntities db = new StudentApplicationSystemEntities())
                {
                    User obj = db.Users.Where(a => a.email.Equals(user.email) && a.password.Equals(user.password)).FirstOrDefault();
                    if (obj != null)
                    {
                        Session["isAdmin"] = obj.isAdmin;
                        Session["UserID"] = obj.userId.ToString();
                        Session["userId"] = obj.userId;
                        Session["UserName"] = obj.email.ToString();

                        if (obj.isAdmin == 0)
                        {
                            Session["Name"] = obj.name.ToString() + " " + obj.surname.ToString();
                        }
                        
                        return RedirectToAction("Index");
                    }
                    
                    return View("LoginFail");
                }
            }
            else
            {
                return HttpNotFound();
            }
                
        }

        public ActionResult NotAuthorized()
        {
            return View();
        }

    }
}
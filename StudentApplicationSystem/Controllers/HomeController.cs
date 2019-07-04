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
        public ActionResult Login([Bind(Include = "email,password")] User user)
        {
            System.Diagnostics.Debug.WriteLine("Debugging Login");
            
            if (ModelState.IsValid)
            {
                using (StudentApplicationSystemDBEntities db = new StudentApplicationSystemDBEntities())
                {
                    User obj = db.Users.Where(a => a.email.Equals(user.email) && a.password.Equals(user.password)).FirstOrDefault();
                    System.Diagnostics.Debug.WriteLine(obj.password);
                    if (obj != null)
                    {
                        Session["UserID"] = obj.userId.ToString();
                        Session["UserName"] = obj.email.ToString();
                        Session["userId"] = obj.userId;
                        return RedirectToAction("Index");
                    }
                    return HttpNotFound();
                }
            }
            else
            {
                return HttpNotFound();
            }
                
        }

    }
}
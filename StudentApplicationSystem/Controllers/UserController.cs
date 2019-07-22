using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using StudentApplicationSystem.Models;

namespace StudentApplicationSystem.Controllers
{
    public class UserController : Controller
    {
        private StudentApplicationSystemEntities db = new StudentApplicationSystemEntities();

        // GET: User
        public ActionResult Index()
        {
            if(CheckVisitor())
            {
                // If a visitor wants to see UserList.
                return RedirectToAction("NotAuthorized", "Home"); ;
            }
            else if(!CheckAdmin())
            {
                // If non-admin user wants to see UserList.
                return RedirectToAction("NotAuthorized", "Home"); ;
            }

            return View(db.Users.ToList());
        }

        // GET: User/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                // If id parameter sent wrong.
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (CheckVisitor())
            {
                // If the user is not logged in.
                return RedirectToAction("NotAuthorized", "Home");
            }
            else if (!CheckAdmin() && id != (int)Session["userId"])
            {
                // If user wants to see somebody elses' details.
                return RedirectToAction("NotAuthorized", "Home");

            }
            else
            {
                // If user is admin or non-admin user wants to see his/her own details.
                User user = db.Users.Find(id);
                if (user == null)
                {
                    // If there is no user with given ID.
                    return HttpNotFound();
                }
                return View(user);
            }
        }

        // GET: User/Create
        public ActionResult Create()
        {
            if (Session["userName"] != null)
            {
                // If logged in user wants to create another user.
                if ((int)Session["isAdmin"] == 0)
                {
                    // If this user is not admin.
                    return RedirectToAction("NotAuthorized", "Home");

                }
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(User user)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    // Check for email is already in use or not.
                    bool alreadyexists = false;

                    // Iterate in Database
                    foreach (User u in db.Users.ToList())
                    {
                        if (u.email.Equals(user.email))
                        {
                            // If already in use break the loop and change match data.
                            alreadyexists = true;
                            break;
                        }
                    }

                    if (alreadyexists)
                    {
                        // In case of duplicate emails, produce an error message and return back to register page.
                        ModelState.AddModelError(string.Empty, "This email already in use.");
                        return View();
                    }

                    if (Session["userName"] == null)
                    {
                        // If create accesed by register.
                        user.BoolValue = false;
                    }

                    // Creating time log stored.
                    user.dt_created = DateTime.Now;
                    db.Users.Add(user);
                    db.SaveChanges();
                }
                catch (DbEntityValidationException e)
                {
                    foreach (var eve in e.EntityValidationErrors)
                    {
                        Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                            eve.Entry.Entity.GetType().Name, eve.Entry.State);
                        foreach (var ve in eve.ValidationErrors)
                        {
                            Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                                ve.PropertyName, ve.ErrorMessage);
                        }
                    }
                    throw;
                }
                return RedirectToAction("Index", "Home", null);
            }

            return View(user);
        }




        // GET: User/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                // Checks ID parameter passed right.
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (CheckVisitor())
            {
                // If non user person wants to reach Edit page.
                return RedirectToAction("NotAuthorized", "Home");
            }
            else
            {
                if (!CheckAdmin() && (int)Session["userId"] != id)
                {
                    // If user is not an admin and not editing (her/him)self.
                    return RedirectToAction("NotAuthorized", "Home");
                }
            }

            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }
        
        // POST: User/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(User user)
        {
            if (ModelState.IsValid)
            {
                // Check for email is already in use or not.
                bool alreadyexists = false;

                // Iterate in Database
                foreach (User u in db.Users.ToList())
                {
                    if (u.email.Equals(user.email) && !(user.userId.Equals(u.userId)))
                    {                        // If already in use break the loop and change match data.
                        alreadyexists = true;
                        break;
                    }
                }

                if (alreadyexists)
                {
                    // In case of duplicate emails, produce an error message and return back to register page.
                    ModelState.AddModelError(string.Empty, "This email already in use.");
                    return RedirectToAction("Edit", new { id = user.userId });
                }

                if ((int)Session["isAdmin"] != 1)
                    user.BoolValue = false;
                user.cd_modifier = (int)Session["userId"];
                user.dt_modified = DateTime.Now;
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                if((int)Session["isAdmin"] != 1)
                {
                    return RedirectToAction("Details", new { id = user.userId });
                }
                return RedirectToAction("Index");
            }
            return View(user);
        }

        // GET: User/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                // Checks ID parameter passed right.
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if(CheckVisitor())
            {
                // If non-user person wants to reach delete action.
                return RedirectToAction("NotAuthorized", "Home");
            }
            else
            {
                // If there is logged in user.
                if (!CheckAdmin())
                {
                    // If the user is not admin.
                    return RedirectToAction("NotAuthorized", "Home");
                }
            }
            
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            User user = db.Users.Find(id);
            db.Users.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }


        public bool CheckVisitor()
        {
            //If visitor return true, if user return false.
            return Session["userName"] == null ? true : false;
        }

        public bool CheckAdmin()
        {
            // If admin return true else false.
            return (int)Session["isAdmin"] == 1 ? true : false;
        }

    }
}

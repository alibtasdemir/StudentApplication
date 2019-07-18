using StudentApplicationSystem.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace StudentApplicationSystem.Controllers
{
    public class CategoriesController : Controller
    {
        private StudentApplicationSystemEntities db = new StudentApplicationSystemEntities();
        // GET: Categories
        public ActionResult Index()
        {
            if (Session["userName"] == null)
            {
                // If non user person wants to reach Edit page.
                return RedirectToAction("NotAuthorized", "Home");
            }
            if ((int)Session["isAdmin"] == 0)
            {
                return RedirectToAction("NotAuthorized", "Home");
            }
            return View(db.Categories.ToList());
        }

        public ActionResult Create()
        {
            if (Session["userName"] == null)
            {
                // If non user person wants to reach Edit page.
                return RedirectToAction("NotAuthorized", "Home");
            }
            if ((int)Session["isAdmin"] == 0)
            {
                return RedirectToAction("NotAuthorized", "Home");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "categoryId,categoryName")] Category category)
        {
            if (Session["userName"] == null)
            {
                // If non user person wants to reach Edit page.
                return RedirectToAction("NotAuthorized", "Home");
            }
            if ((int)Session["isAdmin"] == 0)
            {
                return RedirectToAction("NotAuthorized", "Home");
            }
            if (ModelState.IsValid)
            {
                db.Categories.Add(category);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(category);
        }

        public ActionResult Edit(int? id)
        {
            if (Session["userName"] == null)
            {
                // If non user person wants to reach Edit page.
                return RedirectToAction("NotAuthorized", "Home");
            }
            if ((int)Session["isAdmin"] == 0)
            {
                return RedirectToAction("NotAuthorized", "Home");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Category category = db.Categories.Find(id);
            if(category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "categoryId,categoryName")] Category category)
        {
            if (Session["userName"] == null)
            {
                // If non user person wants to reach Edit page.
                return RedirectToAction("NotAuthorized", "Home");
            }
            if ((int)Session["isAdmin"] == 0)
            {
                return RedirectToAction("NotAuthorized", "Home");
            }
            if (ModelState.IsValid)
            {
                db.Entry(category).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(category);
        }

        public ActionResult Delete(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (Session["userName"] == null)
            {
                // If non user person wants to reach Edit page.
                return RedirectToAction("NotAuthorized", "Home");
            }
            if ((int)Session["isAdmin"] == 0)
            {
                return RedirectToAction("NotAuthorized", "Home");
            }
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        // POST: News/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (Session["userName"] == null)
            {
                // If non user person wants to reach Edit page.
                return RedirectToAction("NotAuthorized", "Home");
            }
            if ((int)Session["isAdmin"] == 0)
            {
                return RedirectToAction("NotAuthorized", "Home");
            }
            Category category = db.Categories.Find(id);
            db.Categories.Remove(category);
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
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using StudentApplicationSystem.Models;

namespace StudentApplicationSystem.Controllers
{
    public class ApplicationsController : Controller
    {
        private StudentApplicationSystemEntities db = new StudentApplicationSystemEntities();
        private int jobId;

        // GET: Applications
        public ActionResult Index()
        {
            int userid = (int)Session["userId"];
            
            User user = db.Users.Where(a => a.userId.Equals(userid)).FirstOrDefault();

            ICollection<Application> applications = user.Applications;
            
            return View(applications);
        }

        // GET: Applications/Details/5
        public ActionResult Details(int? id)
        {
            // Check if id passed right.
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            int userid = (int)Session["userId"];
            User user = db.Users.Where(a => a.userId.Equals(userid)).FirstOrDefault();
            Application application = db.Applications.Find(id);

            if (application == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Check if user is admin or has right to see the application.
            return user.isAdmin == 1 || user.Applications.Contains(application)
                ? View(application)
                : (ActionResult)new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        // GET: Applications/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Applications/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "applicationId,cv,paperId")] Application application)
        {
            if (ModelState.IsValid)
            {
                application.userId = (int)Session["userId"];
                User user = db.Users.Where(a => a.userId.Equals(application.userId)).FirstOrDefault();
                application.dt_created = DateTime.Now;
                user.Applications.Add(application);
                application.jobId = (int)Session["jobId"];
                db.Applications.Add(application);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.userId = new SelectList(db.Users, "userId", "name", application.userId);
            return View(application);
        }

        // GET: Applications/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Application application = db.Applications.Find(id);
            if (application == null)
            {
                return HttpNotFound();
            }
            return View(application);
        }

        // POST: Applications/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "applicationId,userId,cv,paperId,dt_created")] Application application)
        {
            if (ModelState.IsValid)
            {
                db.Entry(application).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(application);
        }

        // GET: Applications/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Application application = db.Applications.Find(id);
            if (application == null)
            {
                return HttpNotFound();
            }
            return View(application);
        }

        // POST: Applications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Application application = db.Applications.Find(id);
            db.Applications.Remove(application);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public int getJobId(int jobId)
        {
            this.jobId = jobId;
            return 1;
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

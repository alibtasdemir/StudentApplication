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
    public class ReviewsController : Controller
    {
        private StudentApplicationSystemEntities db = new StudentApplicationSystemEntities();

        // GET: Reviews
        public ActionResult Index()
        {
            var reviews = db.Reviews.Include(r => r.Application).Include(r => r.InterviewQuestionPaper).Include(r => r.Job).Include(r => r.User);
            return View(reviews.ToList());
        }

        // GET: Reviews/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Review review = db.Reviews.Find(id);
            if (review == null)
            {
                return HttpNotFound();
            }
            return View(review);
        }

        // GET: Reviews/Create
        public ActionResult Create()
        {
            ViewBag.applicationId = new SelectList(db.Applications, "applicationId", "status");
            ViewBag.paperId = new SelectList(db.InterviewQuestionPapers, "paperId", "answer1");
            ViewBag.jobId = new SelectList(db.Jobs, "jobId", "applicantList");
            ViewBag.userId = new SelectList(db.Users, "userId", "name");
            return View();
        }

        // POST: Reviews/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "reviewId,review1,status,interviewDate")] Review review)
        {
            if (ModelState.IsValid)
            {
                review.dt_created = DateTime.Now;
                review.applicationId = (int)Session["applicationId"];
                review.cd_creater = (int)Session["userId"];

                Application application = db.Applications.Find(review.applicationId);
                review.userId = application.userId;
                review.paperId = application.paperId;
                review.jobId = application.jobId;

                application.status = review.status;
                
                db.Reviews.Add(review);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.applicationId = new SelectList(db.Applications, "applicationId", "status", review.applicationId);
            ViewBag.paperId = new SelectList(db.InterviewQuestionPapers, "paperId", "answer1", review.paperId);
            ViewBag.jobId = new SelectList(db.Jobs, "jobId", "applicantList", review.jobId);
            ViewBag.userId = new SelectList(db.Users, "userId", "name", review.userId);
            return View(review);
        }

        public List<SelectListItem> StatusList()
        {
            List<SelectListItem> selectlist = new List<SelectListItem>();
            List<string> DList = new List<string>();
            DList.Add("Accepted");
            DList.Add("Denied");

            DList.Sort();

            foreach (string status in DList)
            {
                selectlist.Add(new SelectListItem
                {
                    Text = status,
                    Value = status
                });
            }

            return selectlist;

        }

        // GET: Reviews/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Review review = db.Reviews.Find(id);
            if (review == null)
            {
                return HttpNotFound();
            }
            List<SelectListItem> items = StatusList();
            ViewData["StatusItems"] = items;
            ViewBag.applicationId = new SelectList(db.Applications, "applicationId", "status", review.applicationId);
            ViewBag.paperId = new SelectList(db.InterviewQuestionPapers, "paperId", "answer1", review.paperId);
            ViewBag.jobId = new SelectList(db.Jobs, "jobId", "applicantList", review.jobId);
            ViewBag.userId = new SelectList(db.Users, "userId", "name", review.userId);
            return View(review);
        }

        // POST: Reviews/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "reviewId,userId,paperId,jobId,applicationId,review1,status,interviewDate,dt_created,dt_modified,cd_creater,cd_modifier")] Review review)
        {
            if (ModelState.IsValid)
            {
                review.dt_modified = DateTime.Now;
                review.cd_modifier = (int)Session["userId"];
                Application application = db.Applications.Find(review.applicationId);

                application.status = review.status;

                db.Entry(review).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", "Applications");
            }
            ViewBag.applicationId = new SelectList(db.Applications, "applicationId", "status", review.applicationId);
            ViewBag.paperId = new SelectList(db.InterviewQuestionPapers, "paperId", "answer1", review.paperId);
            ViewBag.jobId = new SelectList(db.Jobs, "jobId", "applicantList", review.jobId);
            ViewBag.userId = new SelectList(db.Users, "userId", "name", review.userId);
            return View(review);
        }

        // GET: Reviews/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Review review = db.Reviews.Find(id);
            if (review == null)
            {
                return HttpNotFound();
            }
            return View(review);
        }

        // POST: Reviews/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Review review = db.Reviews.Find(id);
            db.Reviews.Remove(review);
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

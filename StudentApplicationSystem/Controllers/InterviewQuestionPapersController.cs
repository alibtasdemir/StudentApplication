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
    public class InterviewQuestionPapersController : Controller
    {
        private StudentApplicationSystemEntities db = new StudentApplicationSystemEntities();

        // GET: InterviewQuestionPapers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (CheckVisitor())
            {
                return RedirectToAction("NotAuthorized", "Home");
            }
            InterviewQuestionPaper interviewQuestionPaper = db.InterviewQuestionPapers.Find(id);
            if (interviewQuestionPaper == null)
            {
                return HttpNotFound();
            }
            return View(interviewQuestionPaper);
            
        }

        // GET: InterviewQuestionPapers/Create
        public ActionResult Create()
        {
            if (CheckVisitor())
            {
                return RedirectToAction("NotAuthorized", "Home");
            }
            if (!CheckAdmin())
            {
                return RedirectToAction("NotAuthorized", "Home");
            }
            ViewBag.applicationId = new SelectList(db.Applications, "applicationId", "applicationId");
            ViewBag.userId = new SelectList(db.Users, "userId", "name");
            ViewBag.question1 = new SelectList(db.Questions, "questionId", "question1");
            ViewBag.question1 = new SelectList(db.Questions, "questionId", "question1");
            ViewBag.question1 = new SelectList(db.Questions, "questionId", "question1");
            return View();
        }

        // POST: InterviewQuestionPapers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "paperId,applicationId,userId,jobId,cd_creater,dt_created,cd_modifier,dt_modified")] InterviewQuestionPaper interviewQuestionPaper)
        {
            if (ModelState.IsValid)
            {
                db.InterviewQuestionPapers.Add(interviewQuestionPaper);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.applicationId = new SelectList(db.Applications, "applicationId", "applicationId", interviewQuestionPaper.applicationId);
            ViewBag.userId = new SelectList(db.Users, "userId", "name", interviewQuestionPaper.userId);
            return View(interviewQuestionPaper);
        }

        // GET: InterviewQuestionPapers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (CheckVisitor())
            {
                return RedirectToAction("NotAuthorized", "Home");
            }
            
            InterviewQuestionPaper interviewQuestionPaper = db.InterviewQuestionPapers.Find(id);
            if (interviewQuestionPaper == null)
            {
                return HttpNotFound();
            }

            if (!CheckAdmin() && (interviewQuestionPaper.userId != (int)Session["userId"]))
            {
                return RedirectToAction("NotAuthorized", "Home");
            }

            if (interviewQuestionPaper.dt_modified != null)
            {
                return RedirectToAction("Details", new { id });
            }

            ViewBag.applicationId = new SelectList(db.Applications, "applicationId", "applicationId", interviewQuestionPaper.applicationId);
            ViewBag.userId = new SelectList(db.Users, "userId", "name", interviewQuestionPaper.userId);
            return View(interviewQuestionPaper);
        }

        // POST: InterviewQuestionPapers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "paperId,applicationId,userId,jobId,answers,cd_creater,dt_created,cd_modifier,dt_modified,questionList")] InterviewQuestionPaper interviewQuestionPaper)
        {
            if (ModelState.IsValid)
            {
                int applicationId = (int)interviewQuestionPaper.applicationId;
                Application application = db.Applications.Find(applicationId);
                application.status = "Pending";
                interviewQuestionPaper.dt_modified = DateTime.Now;
                interviewQuestionPaper.cd_modifier = (int)Session["userId"];  
                db.Entry(interviewQuestionPaper).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", "Applications");
            }
            ViewBag.applicationId = new SelectList(db.Applications, "applicationId", "applicationId", interviewQuestionPaper.applicationId);
            ViewBag.userId = new SelectList(db.Users, "userId", "name", interviewQuestionPaper.userId);
            return View(interviewQuestionPaper);
        }

        // GET: InterviewQuestionPapers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (CheckVisitor())
            {
                return RedirectToAction("NotAuthorized", "Home");
            }
            if (!CheckAdmin())
            {
                return RedirectToAction("NotAuthorized", "Home");
            }
            InterviewQuestionPaper interviewQuestionPaper = db.InterviewQuestionPapers.Find(id);
            if (interviewQuestionPaper == null)
            {
                return HttpNotFound();
            }
            return View(interviewQuestionPaper);
        }

        // POST: InterviewQuestionPapers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            InterviewQuestionPaper interviewQuestionPaper = db.InterviewQuestionPapers.Find(id);
            db.InterviewQuestionPapers.Remove(interviewQuestionPaper);
            db.SaveChanges();
            return RedirectToAction("Index");
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

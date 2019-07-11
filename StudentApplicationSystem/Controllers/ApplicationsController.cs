using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.IO;
using System.Web;
using System.Web.Mvc;
using StudentApplicationSystem.Models;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;

namespace StudentApplicationSystem.Controllers
{
    public class ApplicationsController : Controller
    {
        
        private StudentApplicationSystemEntities db = new StudentApplicationSystemEntities();
        private int jobId;

        // GET: Applications
        public ActionResult Index()
        {
            if (CheckVisitor())
            {
                // If non user person wants to reach Edit page.
                return RedirectToAction("NotAuthorized", "Home");
            }

            int userid = (int)Session["userId"];
            
            var user = db.Users.Where(a => a.userId.Equals(userid)).FirstOrDefault();
            if(user.isAdmin == 1)
            {
                return View(db.Applications.ToList());
            }
            var applications = user.Applications;
            
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
            if (CheckVisitor())
            {
                // If non user person wants to reach Edit page.
                return RedirectToAction("NotAuthorized", "Home");
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
        public ActionResult Create([Bind(Include = "applicationId,cvFile,paperId")] Application application)
        {
            
            if (ModelState.IsValid)
            {
                if (application.cvFile != null && application.cvFile.ContentLength > 0)
                {
                    string FileExtension = Path.GetExtension(application.cvFile.FileName).ToUpper();

                    if (FileExtension == ".PDF")
                    {
                        byte[] bytes;
                        using (BinaryReader br = new BinaryReader(application.cvFile.InputStream))
                        {
                            bytes = br.ReadBytes(application.cvFile.ContentLength);
                        }
                        
                        application.cv = bytes;
                    }
                }
                application.userId = (int)Session["userId"];
                application.status = "Pending";
                User user = db.Users.Where(a => a.userId == (application.userId)).FirstOrDefault();
                application.dt_created = DateTime.Now;
                user.Applications.Add(application);

                application.jobId = (int)Session["jobId"];

                Job job = db.Jobs.Find(application.jobId);
                
                var questionIds = db.Questions.OrderBy(h => Guid.NewGuid()).Select(c => c.questionId).Take(3).ToList();

                InterviewQuestionPaper paper = new InterviewQuestionPaper();
                paper.question1 = questionIds[0];
                paper.question2 = questionIds[1];
                paper.question3 = questionIds[2];

                paper.userId = application.userId;
                paper.jobId = application.jobId;
                paper.dt_created = DateTime.Now;
                paper.cd_creater = application.userId;
                
                db.InterviewQuestionPapers.Add(paper);
                db.SaveChanges();

                application.paperId = paper.paperId;
                paper.applicationId = application.applicationId;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.userId = new SelectList(db.Users, "userId", "name", application.userId);
            return View(application);
        }

        [HttpGet]
        public FileResult Download(int? id)
        {
            Application application = db.Applications.Find(id);
            return File(application.cv, ".pdf");

        }
        
        // GET: Applications/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (CheckVisitor())
            {
                // If non user person wants to reach Edit page.
                return RedirectToAction("NotAuthorized", "Home");
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
            int? paperId = application.paperId;
            InterviewQuestionPaper interview = db.InterviewQuestionPapers.Find(paperId);
            db.InterviewQuestionPapers.Remove(interview);
            db.Applications.Remove(application);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Review(int? id)
        {
            Review review = new Review
            {
                dt_created = DateTime.Now,
                applicationId = id,
                cd_creater = (int)Session["userId"]
            };

            Application application = db.Applications.Find(review.applicationId);
            review.userId = application.userId;
            review.paperId = application.paperId;
            review.jobId = application.jobId;

            db.Reviews.Add(review);
            db.SaveChanges();

            return RedirectToAction("Edit", "Reviews", new { id = review.reviewId });
        }

        public int getJobId(int jobId)
        {
            this.jobId = jobId;
            return 1;
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

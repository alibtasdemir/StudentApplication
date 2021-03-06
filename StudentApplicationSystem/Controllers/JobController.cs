﻿using System;
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
    public class JobController : Controller
    {
        private StudentApplicationSystemEntities db = new StudentApplicationSystemEntities();

        // GET: Job
        public ActionResult Index()
        {
            if(CheckVisitor())
            {
                // No visitor access.
                return RedirectToAction("NotAuthorized", "Home");
            }
            return View(db.Jobs.ToList());
        }

        // GET: Job/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (CheckVisitor())
            {
                // No visitor access.
                return RedirectToAction("NotAuthorized", "Home");
            }
            Job job = db.Jobs.Find(id);
            if (job == null)
            {
                return HttpNotFound();
            }
            return View(job);
        }

        // GET: Job/Create
        public ActionResult Create()
        {
            if (CheckVisitor())
            {
                // No visitor access.
                return RedirectToAction("NotAuthorized", "Home");
            }
            if (!CheckAdmin())
            {
                return RedirectToAction("NotAuthorized", "Home");
            }
            List<SelectListItem> items = CategoryList();
            ViewBag.Categories = items;
            return View();
        }

        // POST: Job/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "jobId,applicantList,jobName,applicationStart,applicationFinish,SelectedCategories,questionNumber")] Job job)
        {
            if (ModelState.IsValid)
            {
                job.categories = string.Join(",", job.SelectedCategories);
                db.Jobs.Add(job);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(job);
        }

        // GET: Job/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (CheckVisitor())
            {
                // No visitor access.
                return RedirectToAction("NotAuthorized", "Home");
            }
            if (!CheckAdmin())
            {
                return RedirectToAction("NotAuthorized", "Home");
            }
            Job job = db.Jobs.Find(id);
            if (job == null)
            {
                return HttpNotFound();
            }
            return View(job);
        }

        // POST: Job/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "jobId,applicantList,jobName,applicationStart,applicationFinish,questionNumber")] Job job)
        {
            if (ModelState.IsValid)
            {
                db.Entry(job).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(job);
        }

        // GET: Job/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (CheckVisitor())
            {
                // No visitor access.
                return RedirectToAction("NotAuthorized", "Home");
            }
            if (!CheckAdmin())
            {
                return RedirectToAction("NotAuthorized", "Home");
            }
            Job job = db.Jobs.Find(id);
            if (job == null)
            {
                return HttpNotFound();
            }
            return View(job);
        }

        // POST: Job/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Job job = db.Jobs.Find(id);
            var applications = db.Applications.Where(a => a.jobId == (job.jobId)).ToList();

            foreach (var app in applications)
            {
                Review review = db.Reviews.Where(a => a.applicationId == (app.applicationId)).FirstOrDefault();
                if(review != null)
                {
                    db.Reviews.Remove(review);
                }
                

                InterviewQuestionPaper paper = db.InterviewQuestionPapers.Find(app.paperId);

                if(paper != null)
                {
                    app.paperId = null;
                    paper.applicationId = null;
                
                    db.SaveChanges();
                    db.InterviewQuestionPapers.Remove(paper);
                    db.Applications.Remove(app);
                }
                else
                {
                    db.SaveChanges();
                    db.Applications.Remove(app);
                }


            }


            db.Jobs.Remove(job);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Apply(int? id)
        {
            Session["jobId"] = id;
            return RedirectToAction("Create", "Applications");
        }


        public List<SelectListItem> CategoryList()
        {
            List<SelectListItem> selectlist = new List<SelectListItem>();
            List<Category> DList = new List<Category>();

            foreach(var category in db.Categories.ToList())
            {
                DList.Add(category);
            }
            

            foreach (var category in DList)
            {
                selectlist.Add(new SelectListItem
                {
                    Text = category.categoryName,
                    Value = category.categoryId.ToString()
                });
            }

            return selectlist;

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

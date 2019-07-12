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
    public class QuestionController : Controller
    {
        private StudentApplicationSystemEntities db = new StudentApplicationSystemEntities();

        // GET: Question
        public ActionResult Index()
        {
            if(CheckVisitor())
            {
                // If non-user wants to reach question poll.
                return RedirectToAction("NotAuthorized", "Home");
            }
            else if(!CheckAdmin())
            {
                // If normal user wants to reach question poll.
                return RedirectToAction("NotAuthorized", "Home");
            }            
            return View(db.Questions.ToList());
        }

        // GET: Question/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (CheckVisitor())
            {
                // If non-user wants to reach question details.
                return RedirectToAction("NotAuthorized", "Home");
            }
            else if (!CheckAdmin())
            {
                // If normal user wants to reach question details.
                return RedirectToAction("NotAuthorized", "Home");
            }
            Question question = db.Questions.Find(id);
            if (question == null)
            {
                return HttpNotFound();
            }
            return View(question);
        }

        // GET: Question/Create
        public ActionResult Create()
        {
            if (CheckVisitor())
            {
                // If non-user wants to reach question create.
                return RedirectToAction("NotAuthorized", "Home");
            }
            else if (!CheckAdmin())
            {
                // If normal user wants to reach question create.
                return RedirectToAction("NotAuthorized", "Home");
            }
            return View();
        }

        // POST: Question/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "question1")] Question question)
        {
            if (ModelState.IsValid)
            {
                question.cd_creater = (int)Session["userId"];
                question.dt_created = DateTime.Now;
                db.Questions.Add(question);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(question);
        }

        // GET: Question/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (CheckVisitor())
            {
                // If non-user wants to reach question edit.
                return RedirectToAction("NotAuthorized", "Home");
            }
            else if (!CheckAdmin())
            {
                // If normal user wants to reach question edit.
                return RedirectToAction("NotAuthorized", "Home");
            }
            Question question = db.Questions.Find(id);
            if (question == null)
            {
                return HttpNotFound();
            }
            return View(question);
        }

        // POST: Question/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "questionId,question1,cd_creater,dt_created")] Question question)
        {
            if (ModelState.IsValid)
            {
                question.cd_modifier = (int)Session["userId"];
                question.dt_modified = DateTime.Now;
                db.Entry(question).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(question);
        }

        // GET: Question/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (CheckVisitor())
            {
                // If non-user wants to reach question edit.
                return RedirectToAction("NotAuthorized", "Home");
            }
            else if (!CheckAdmin())
            {
                // If normal user wants to reach question edit.
                return RedirectToAction("NotAuthorized", "Home");
            }
            Question question = db.Questions.Find(id);
            if (question == null)
            {
                return HttpNotFound();
            }
            return View(question);
        }

        // POST: Question/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Question question = db.Questions.Find(id);
            db.Questions.Remove(question);
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

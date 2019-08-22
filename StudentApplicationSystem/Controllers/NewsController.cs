using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using StudentApplicationSystem.Models;

namespace StudentApplicationSystem.Controllers
{
    public class NewsController : Controller
    {
        private StudentApplicationSystemEntities db = new StudentApplicationSystemEntities();

        // GET: News
        public ActionResult Index()
        {
            return View(db.News.ToList());
        }

        // GET: News/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            New @new = db.News.Find(id);
            if (@new == null)
            {
                return HttpNotFound();
            }
            return View(@new);
        }

        // GET: News/Create
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

        // POST: News/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "newId,header,text,imageFile,BoolValue,featuredList,dt_created,dt_modified,cd_creater,cd_modifier")] New @new)
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
                if (@new.imageFile != null && @new.imageFile.ContentLength > 0)
                {
                    string FileExtension = Path.GetExtension(@new.imageFile.FileName).ToUpper();

                    if (FileExtension == ".JPG" || FileExtension == ".PNG" || FileExtension == ".JPEG")
                    {
                        byte[] bytes;
                        using (BinaryReader br = new BinaryReader(@new.imageFile.InputStream))
                        {
                            bytes = br.ReadBytes(@new.imageFile.ContentLength);
                        }

                        @new.image = bytes;
                    }
                }

                @new.dt_created = DateTime.Now;
                @new.cd_creater = (int)Session["userId"];
                db.News.Add(@new);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(@new);
        }

        // GET: News/Edit/5
        public ActionResult Edit(int? id)
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
            New @new = db.News.Find(id);
            if (@new == null)
            {
                return HttpNotFound();
            }
            return View(@new);
        }

        // POST: News/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "newId,header,text,image,BoolValue,featuredList,dt_created,dt_modified,cd_creater,cd_modifier")] New @new)
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
                @new.dt_modified = DateTime.Now;
                @new.cd_modifier = (int)Session["userId"];
                db.Entry(@new).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(@new);
        }

        // GET: News/Delete/5
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
            New @new = db.News.Find(id);
            if (@new == null)
            {
                return HttpNotFound();
            }
            return View(@new);
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
            New @new = db.News.Find(id);
            db.News.Remove(@new);
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

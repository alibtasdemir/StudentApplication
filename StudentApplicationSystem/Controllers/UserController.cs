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
            // Departments list for dropdown, prepared before Register form is rendered. 
            List<SelectListItem> items = DepartmentList();
            ViewData["ListItems"] = items;
            return View();
        }

        // POST: User/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "name,surname,password,email,department,gpa,phone_number,BoolValue")] User user)
        {
            
            if (ModelState.IsValid)
            {
                try
                {
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
            List<SelectListItem> items = DepartmentList();
            ViewData["ListItems"] = items;
            return View(user);
        }

        // POST: User/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "userId,name,surname,email,department,gpa,phone_number,password,dt_created,BoolValue")] User user)
        {
            if (ModelState.IsValid)
            {
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

        public List<SelectListItem> DepartmentList()
        {
            List<SelectListItem> selectlist = new List<SelectListItem>();
            List<string> DList = new List<string>();
            DList.Add("Faculty of Computer and Information Sciences - Computer Engineering");
            DList.Add("Faculty of Computer and Information Sciences - Information Systems Engineering");
            DList.Add("Faculty of Computer and Information Sciences - Software Engineering");

            DList.Add("Faculty of Education - Computer Education and Instructional Technologies");
            DList.Add("Faculty of Education - Foreign Language Education");
            DList.Add("Faculty of Education - Special Education");
            DList.Add("Faculty of Education - Science Education Department");
            DList.Add("Faculty of Education - Mathematics Education Department");
            DList.Add("Faculty of Education - Social Sciences Education Department");
            DList.Add("Faculty of Education - Faculty of Education");

            DList.Add("Faculty of Dentistry - Dentistry");

            DList.Add("Faculty of Arts and Sciences - Turkish Language and Literature");
            DList.Add("Faculty of Arts and Sciences - History");
            DList.Add("Faculty of Arts and Sciences - Sociology");
            DList.Add("Faculty of Arts and Sciences - Social Work");
            DList.Add("Faculty of Arts and Sciences - History of Art");
            DList.Add("Faculty of Arts and Sciences - Mathematics");
            DList.Add("Faculty of Arts and Sciences - Chemistry");
            DList.Add("Faculty of Arts and Sciences - Physics");
            DList.Add("Faculty of Arts and Sciences - Philosophy");
            DList.Add("Faculty of Arts and Sciences - Geography");
            DList.Add("Faculty of Arts and Sciences - Translation Studies");
            DList.Add("Faculty of Arts and Sciences - Biology");
            DList.Add("Faculty of Arts and Sciences - German Language and Literature");

            DList.Add("Faculty of Law - Law");

            DList.Add("Faculty of Theology - Theology");

            DList.Add("Faculty of Communication - Public Relations and Advertising");
            DList.Add("Faculty of Communication - Communication Design and Media");
            DList.Add("Faculty of Communication - Journalism");
            DList.Add("Faculty of Communication - Radio Television and Cinema");

            DList.Add("Sakarya Business School - Human Resources Management");
            DList.Add("Sakarya Business School - Business");
            DList.Add("Sakarya Business School - Health Administration");
            DList.Add("Sakarya Business School - International Trade");
            DList.Add("Sakarya Business School - Management Information Systems");

            DList.Add("Faculty of Engineering - Environmental Engineering");
            DList.Add("Faculty of Engineering - Civil Engineering");
            DList.Add("Faculty of Engineering - Electrical and Electronics Engineering");
            DList.Add("Faculty of Engineering - Food Engineering");
            DList.Add("Faculty of Engineering - Industrial Engineering");
            DList.Add("Faculty of Engineering - Geophysical Engineering");
            DList.Add("Faculty of Engineering - Mechanical Engineering");
            DList.Add("Faculty of Engineering - Metallurgical and Materials Engineering");

            DList.Add("Faculty of Health Sciences - Midwifery");
            DList.Add("Faculty of Health Sciences - Nursing");

            DList.Add("Faculty of Art Design And Architecture - Visual Communication and Design");
            DList.Add("Faculty of Art Design And Architecture - Traditional Turkish Arts");
            DList.Add("Faculty of Art Design And Architecture - Painting");
            DList.Add("Faculty of Art Design And Architecture - Architecture");
            DList.Add("Faculty of Art Design And Architecture - Ceramic and Glass Design");


            DList.Add("Faculty of Political Sciences - Labour Economics and Industrial Relations");
            DList.Add("Faculty of Political Sciences - Econometrics");
            DList.Add("Faculty of Political Sciences - Economics");
            DList.Add("Faculty of Political Sciences - Political Science and Public Administration");
            DList.Add("Faculty of Political Sciences - Public Finance");
            DList.Add("Faculty of Political Sciences - International Relations");



            DList.Add("Faculty of Technical Education - Electrical Training");
            DList.Add("Faculty of Technical Education - Mechanical Education");
            DList.Add("Faculty of Technical Education - Metal Education");
            DList.Add("Faculty of Technical Education - Construction Education");

            DList.Add("Faculty of Faculty of Medicine - Medicine");

            DList.Sort();

            foreach (string department in DList)
            {
                selectlist.Add(new SelectListItem
                {
                    Text = department,
                    Value = department
                });
            }

            return selectlist;
        
        }
    }
}

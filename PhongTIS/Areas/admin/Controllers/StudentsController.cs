using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PhongTIS.Models;
using PhongTIS.Areas.admin.Common;
namespace PhongTIS.Areas.admin.Controllers
{
    public class StudentsController : Controller
    {
        private PhongTISDB db = new PhongTISDB();

        // GET: admin/Students
        public ActionResult Index()
        {
            var students = db.Students.Include(s => s.Category);
            return View(students.ToList());
        }

        // GET: admin/Students/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // GET: admin/Students/Create
        public ActionResult Create()
        {
            ViewBag.idSubject = new SelectList(db.Categories, "id", "nameCategory");
            return View();
        }

        // POST: admin/Students/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,username,pwd,idSubject")] Student student)
        {
            if (ModelState.IsValid)
            {
                var password = Encryptor.MD5Hash(student.pwd);
                student.id = getMaxId();
                student.pwd = password;
                db.Students.Add(student);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.idSubject = new SelectList(db.Categories, "id", "nameCategory", student.idSubject);
            return View(student);
        }

        // GET: admin/Students/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            ViewBag.idSubject = new SelectList(db.Categories, "id", "nameCategory", student.idSubject);
            return View(student);
        }

        // POST: admin/Students/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,username,pwd,idSubject")] Student student)
        {
            if (ModelState.IsValid)
            {
                var password = Encryptor.MD5Hash(student.pwd);
                student.pwd = password;
                db.Entry(student).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.idSubject = new SelectList(db.Categories, "id", "nameCategory", student.idSubject);
            return View(student);
        }

        // GET: admin/Students/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // POST: admin/Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Student student = db.Students.Find(id);
            db.Students.Remove(student);
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
        public int getMaxId()
        {
            return db.Students.Count();
        }
    }
}

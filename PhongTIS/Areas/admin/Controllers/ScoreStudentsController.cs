using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Drawing.Printing;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PhongTIS.Areas.admin.Dao;
using System.Web.UI;
using PhongTIS.Models;

namespace PhongTIS.Areas.admin.Controllers
{
    public class ScoreStudentsController : Controller
    {
        private PhongTISDB db = new PhongTISDB();

        // GET: admin/ScoreStudents
        public ActionResult Index(int page = 1, int pageSize = 1)
        {
       
            return View(db.ScoreStudents.ToList());
        }

        // GET: admin/ScoreStudents/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ScoreStudent scoreStudent = db.ScoreStudents.Find(id);
            if (scoreStudent == null)
            {
                return HttpNotFound();
            }
            return View(scoreStudent);
        }

        // GET: admin/ScoreStudents/Create
        public ActionResult Create()
        {
            ViewBag.IDTest = new SelectList(db.Exercises, "id", "test");
            ViewBag.IDName = new SelectList(db.Students, "id", "username");
            return View();
        }

        // POST: admin/ScoreStudents/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,IDName,scoreTest,IDTest,meta,hide,order,datebegin")] ScoreStudent scoreStudent)
        {
            if (ModelState.IsValid)
            {
                db.ScoreStudents.Add(scoreStudent);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IDTest = new SelectList(db.Exercises, "id", "test", scoreStudent.IDTest);
            ViewBag.IDName = new SelectList(db.Students, "id", "username", scoreStudent.IDName);
            return View(scoreStudent);
        }

        // GET: admin/ScoreStudents/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ScoreStudent scoreStudent = db.ScoreStudents.Find(id);
            if (scoreStudent == null)
            {
                return HttpNotFound();
            }
            ViewBag.IDTest = new SelectList(db.Exercises, "id", "test", scoreStudent.IDTest);
            ViewBag.IDName = new SelectList(db.Students, "id", "username", scoreStudent.IDName);
            return View(scoreStudent);
        }

        // POST: admin/ScoreStudents/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,IDName,scoreTest,IDTest,meta,hide,order,datebegin")] ScoreStudent scoreStudent)
        {
            if (ModelState.IsValid)
            {
                db.Entry(scoreStudent).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IDTest = new SelectList(db.Exercises, "id", "test", scoreStudent.IDTest);
            ViewBag.IDName = new SelectList(db.Students, "id", "username", scoreStudent.IDName);
            return View(scoreStudent);
        }

        // GET: admin/ScoreStudents/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ScoreStudent scoreStudent = db.ScoreStudents.Find(id);
            if (scoreStudent == null)
            {
                return HttpNotFound();
            }
            return View(scoreStudent);
        }

        // POST: admin/ScoreStudents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ScoreStudent scoreStudent = db.ScoreStudents.Find(id);
            db.ScoreStudents.Remove(scoreStudent);
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

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
using PagedList;
using System.IO;
using System.Data.Entity.Validation;
using System.Text;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using IronSoftware.Pdfium;
using iText.Kernel.Pdf.Canvas.Parser.Data;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using System.Web.UI.WebControls;

namespace PhongTIS.Areas.admin.Controllers
{
    public class ExercisesController : Controller
    {
        private PhongTISDB db = new PhongTISDB();
        // GET: admin/Exercises
        public ActionResult Index(long? id)
        {
            getCategory(id);
            return View();
        }
        public void getCategory(long? selectedId = null)
        {
            ViewBag.Category = new SelectList(db.Categories.Where(x => x.hide == true).OrderBy(x => x.order), "id", "nameCategory", selectedId);
        }
        public ActionResult getExercise(long? id)
        {
            if (id == null)
            {
                var v = db.Exercises.OrderBy(x => x.CategoryID).ThenBy(y=> y.part).ToList();
                return PartialView(v);
            }
            var m = db.Exercises.Where(x => x.CategoryID == id).OrderBy(x => x.part).ToList();
            return PartialView(m);
        }
        public ActionResult getQuestion(long? id)
        {
            if(id == null)
            {
                var v = db.Questions.OrderBy(x => x.order).ToList();
                return View(v);
            }
            Session["idExercise"] = id;
            var m = db.Questions.Where(x => x.ExerciseID == id).OrderBy(x => x.order).ToList();
            return View(m);
        }
        // GET: admin/Exercises/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Exercise exercise = db.Exercises.Find(id);
            if (exercise == null)
            {
                return HttpNotFound();
            }
            return View(exercise);
        }

        // GET: admin/Exercises/Create
        public ActionResult Create()
        {
            ViewBag.CategoryID = new SelectList(db.Categories, "id", "nameCategory");
            return View();
        }

        // POST: admin/Exercises/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,test,part,timer,CategoryID,meta,hide,order,datebegin,audio")] Exercise exercise, HttpPostedFileBase audio)
        {
            var path = "";
            var filename = "";
            if (ModelState.IsValid)
            {
                if (audio != null)
                {
                    filename = Path.GetFileName(audio.FileName);
                    path = Path.Combine(Server.MapPath("~/Audio"), filename);
                    audio.SaveAs(path);
                    exercise.audio = filename; //Lưu ý
                }
                else
                {
                    exercise.audio = null;
                }
                int order  = db.Exercises.Where(u => u.part == exercise.part).ToList().Count();
                exercise.id = getMaxId()+1;
                Session["idExercise"] = exercise.id;
                string part = exercise.part.Replace(" ","").ToLower();
                string test = exercise.test.Replace(" ", "").ToLower();
                string meta = part + "-" + test;
                Session["metaExercise"] = meta;
                exercise.meta = meta;
                exercise.hide = true;
                exercise.order = order + 1;
                db.Exercises.Add(exercise);
                db.SaveChanges();
                return RedirectToAction("Create","Questions");
            }

            ViewBag.CategoryID = new SelectList(db.Categories, "id", "nameCategory", exercise.CategoryID);
            return View(exercise);
        }

        // GET: admin/Exercises/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Exercise exercise = db.Exercises.Find(id);
            Session["metaExercise"] = exercise.meta;
            if (exercise == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryID = new SelectList(db.Categories, "id", "nameCategory", exercise.CategoryID);
            return View(exercise);
        }

        // POST: admin/Exercises/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,test,part,timer,CategoryID,meta,hide,order,datebegin,audio")] Exercise exercise, HttpPostedFileBase audio)
        {
            try
            {
                var temp = db.Exercises.Find(exercise.id);
                var path = "";
                var filename = "";
                if (ModelState.IsValid)
                {
                    if (audio != null)
                    {
                        filename = Path.GetFileName(audio.FileName);
                        path = Path.Combine(Server.MapPath("~/Audio"), filename);
                        audio.SaveAs(path);
                        temp.audio = filename; //Lưu ý
                    }
                    temp.test = exercise.test;
                    temp.part = exercise.part;
                    temp.timer = exercise.timer;
                    temp.CategoryID = exercise.CategoryID;
                    temp.hide = exercise.hide;
                    temp.order = exercise.order;
                    db.Entry(temp).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index", "Exercises", new { id = exercise.CategoryID });
                }
            }
            catch (DbEntityValidationException e)
            {
                throw e;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            ViewBag.CategoryID = new SelectList(db.Categories, "id", "nameCategory", exercise.CategoryID);
            return View(exercise);
        }

        // GET: admin/Exercises/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Exercise exercise = db.Exercises.Find(id);
            if (exercise == null)
            {
                return HttpNotFound();
            }
            return View(exercise);
        }

        // POST: admin/Exercises/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Exercise exercise = db.Exercises.Find(id);
            List<Question> questionList = db.Questions.Where(x => x.ExerciseID == id).ToList();
            List<ScoreStudent> scoreList = db.ScoreStudents.Where(x => x.IDTest== id).ToList();
            db.Exercises.Remove(exercise);
            foreach (var i in questionList)
            {
                db.Questions.Remove(i);
            }
            foreach(var i in scoreList)
            {
                db.ScoreStudents.Remove(i);
            }
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
            return db.Exercises.Max(u => u.id);
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult ProcessPdf(HttpPostedFileBase file)
        //{
        //    if (file != null && file.ContentLength > 0)
        //    {
        //        try
        //        {
        //            var questionsAndAnswers = ExtractQuestionsAndAnswers(file.InputStream);

        //            // Now you can do something with the extracted data, like save it to a database or display it in the view.
        //            return View("Result", questionsAndAnswers);
        //        }
        //        catch (Exception ex)
        //        {
        //            ViewBag.Error = "An error occurred while processing the PDF: " + ex.Message;
        //            return View("Index");
        //        }
        //    }

        //    ViewBag.Error = "Please choose a valid PDF file.";
        //    return View("Index");
        //}
        //private List<KeyValuePair<string, string>> ExtractQuestionsAndAnswers(Stream pdfStream)
        //{
        //    var questionsAndAnswers = new List<KeyValuePair<string, string>>();
        //    using (var pdfReader = new PdfReader(pdfStream))
        //    {
        //        var pdfDocument = new PdfDocument(pdfReader);
        //        var numberOfPages = pdfDocument.GetNumberOfPages();
                
        //        for (int i = 1; i <= numberOfPages; i++)
        //        {
        //            var page = pdfDocument.GetPage(i);
        //            var strategy = new iText.Kernel.Pdf.Canvas.Parser.Listener.LocationTextExtractionStrategy();
        //            var currentText = PdfTextExtractor.GetTextFromPage(page);
        //            var lines = currentText.Split('\n');

        //            var questionBuilder = new StringBuilder();
        //            var answerBuilder = new StringBuilder();
        //            var counter = 0;
                    
        //            foreach (var line in lines)
        //            {
        //                var str = "";
        //                counter += 1;
        //                if(counter % 2 != 0)
        //                {
        //                    str = line;
        //                    if(str.IndexOf("  ") != -1)
        //                    {
        //                        str = str.Substring(0, str.IndexOf("  "));
                                
        //                    }
        //                    questionBuilder.AppendLine(str + "///");
        //                }
        //                // You need to implement your logic to distinguish between questions and answers.
        //                // This is a simple example assuming questions end with '?'.

        //                //if (line.Trim().Contains("Section"))
        //                //{
        //                //    questionBuilder.AppendLine(line);
        //                //}
        //                //else
        //                //{
        //                //    answerBuilder.AppendLine(line);
        //                //}
        //            }

        //            questionsAndAnswers.Add(new KeyValuePair<string, string>(
        //                questionBuilder.ToString().Trim(),
        //                answerBuilder.ToString().Trim()
        //            ));
        //        }
        //    }

        //    return questionsAndAnswers;
        //}



    }
}

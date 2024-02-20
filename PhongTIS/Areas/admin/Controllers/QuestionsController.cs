using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using iText.Kernel.Pdf.Canvas.Parser.Data;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf;
using PhongTIS.Models;

namespace PhongTIS.Areas.admin.Controllers
{
    public class QuestionsController : Controller
    {
        private PhongTISDB db = new PhongTISDB();

        // GET: admin/Questions
        public ActionResult Index()
        {
            var questions = db.Questions.Include(q => q.Exercise);
            return View(questions.ToList());
        }

        // GET: admin/Questions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Question question = db.Questions.Find(id);
            if (question == null)
            {
                return HttpNotFound();
            }
            return View(question);
        }

        // GET: admin/Questions/Create
        public ActionResult Create()
        {
            ViewBag.ExerciseID = new SelectList(db.Exercises, "id", "test");
            return View();
        }

        // POST: admin/Questions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Create([Bind(Include = "id,name_question,answer_one,answer_two,answer_three,answer_four,correct_answer,ExerciseID,meta,hide,order,datebegin,img")] Question question, HttpPostedFileBase img)
        {
            try
            {
                var path = "";
                var filename = "";
                var pathImg = "";
                var filenameImg = "";
                if (ModelState.IsValid)
                {
                    if(img != null)
                    {
                        filenameImg = DateTime.Now.ToString("dd-MM-yy-hh-mm-ss-") + img.FileName;
                        pathImg = Path.Combine(Server.MapPath("~/Images"), filenameImg);
                        img.SaveAs(pathImg);
                        question.img = filenameImg; //Lưu ý
                    }
                    else
                    {
                        question.img=null;
                    }

                    List<Question> questions = getMaxOrder();
                    question.id = getMaxId() + 1;
                    question.ExerciseID = Convert.ToInt32(Session["idExercise"]);
                    string meta = Session["metaExercise"].ToString();
                    question.meta = meta;
                    question.hide = true;
                    question.order = questions.Count() + 1;
                    db.Questions.Add(question);
                    db.SaveChanges();
                } 
                return View(question);
            }
            catch (DbEntityValidationException e)
            {
                throw e;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ActionResult getNewQuestion(long? id)
        {
            var m = db.Questions.Where(x => x.ExerciseID == id).OrderBy(x => x.order).ToList();
            return PartialView(m);
        }
        // GET: admin/Questions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Question question = db.Questions.Find(id);
            if (question == null)
            {
                return HttpNotFound();
            }
            ViewBag.ExerciseID = new SelectList(db.Exercises, "id", "test", question.ExerciseID);
            return View(question);
        }

        // POST: admin/Questions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,name_question,answer_one,answer_two,answer_three,answer_four,correct_answer,ExerciseID,meta,hide,order,datebegin,img")] Question question, HttpPostedFileBase img)
        {
            try
            {

                var pathImg = "";
                var filenameImg = "";
                Question temp = getById(question.id);

                if (ModelState.IsValid)
                {
                    
                    if (img != null)
                    {
                        filenameImg = DateTime.Now.ToString("dd-MM-yy-hh-mm-ss-") + img.FileName;
                        pathImg = Path.Combine(Server.MapPath("~/Images"), filenameImg);
                        img.SaveAs(pathImg);
                        temp.img = filenameImg; //Lưu ý
                    }
                    temp.name_question = question.name_question;
                    temp.answer_one= question.answer_one;
                    temp.answer_two= question.answer_two;
                    temp.answer_three = question.answer_three;
                    temp.answer_four= question.answer_four;
                    temp.correct_answer= question.correct_answer;
                    temp.hide = question.hide;
                    temp.order= question.order;
                    temp.datebegin = Convert.ToDateTime(DateTime.Now.ToShortDateString());

                    db.Entry(temp).State = EntityState.Modified;
                    db.SaveChanges();
                }
                return RedirectToAction("getQuestion", "Exercises", new { id = temp.ExerciseID });
            }
            catch (DbEntityValidationException e)
            {
                throw e;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // GET: admin/Questions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Question question = db.Questions.Find(id);
            if (question == null)
            {
                return HttpNotFound();
            }
            return View(question);
        }

        // POST: admin/Questions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Question question = db.Questions.Find(id);
            db.Questions.Remove(question);
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
            return db.Questions.Max(u => u.id);
        }
        public List<Question> getMaxOrder()
        {
            int idExercise = Convert.ToInt32(Session["idExercise"]);
            return db.Questions.Where(u => u.ExerciseID == idExercise).ToList();
        }
        public Question getById(long id)
        {
            return db.Questions.Where(x => x.id == id).FirstOrDefault();

        }
        [HttpPost]
        public ActionResult CreatePartOne([Bind(Include = "id,name_question,answer_one,answer_two,answer_three,answer_four,correct_answer,ExerciseID,meta,hide,order,datebegin,img")] Question question)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    List<Question> questions = getMaxOrder();
                    question.id = getMaxId() + 1;
                    question.answer_one = "A";
                    question.answer_two = "B";
                    question.answer_three = "C";
                    question.answer_four = "D";
                    question.ExerciseID = Convert.ToInt32(Session["idExercise"]);
                    string meta = Session["metaExercise"].ToString();
                    question.meta = meta;
                    question.hide = true;
                    question.order = questions.Count() + 1;
                    db.Questions.Add(question);
                    db.SaveChanges();
                }
                return View("ImageResult");
            }
            catch (DbEntityValidationException e)
            {
                throw e;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost]
        public ActionResult ExtractImages(HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
            {
                try
                {
                    var extractedImages = ExtractAndSaveImagesFromPdf(file.InputStream);
                    Session["ImagePartOne"] = extractedImages;
                    // Now you can do something with the extracted images, like display them or save them.
                    return View("ImageResult");
                    //return View("ImageResult", extractedImages);
                }
                catch (Exception ex)
                {
                    ViewBag.Error = "An error occurred while extracting images: " + ex.Message;
                    return View("Index");
                }
            }

            ViewBag.Error = "Please choose a valid PDF file.";
            return View("Index");
        }
        private List<string> ExtractAndSaveImagesFromPdf(Stream pdfStream)
        {
            var extractedImagePaths = new List<string>();

            using (var pdfReader = new PdfReader(pdfStream))
            using (var pdfDocument = new PdfDocument(pdfReader))
            {
                for (int i = 1; i <= pdfDocument.GetNumberOfPages(); i++)
                {
                    var currentPage = pdfDocument.GetPage(i);
                    var imageListener = new ImageRenderListener();

                    // Extract images from the current page
                    PdfCanvasProcessor processor = new PdfCanvasProcessor(imageListener);
                    processor.ProcessPageContent(currentPage);
                    var count = 0;
                    foreach (var imageBytes in imageListener.Images)
                    {
                        count += 1;
                        if (count != 7 && count != 8 && count != 9)
                        {
                            // Save each image to the disk
                            var filename = $"Image_{Guid.NewGuid()}.png";
                            var imagePath = Path.Combine(Server.MapPath("~/Images"), filename);

                            System.IO.File.WriteAllBytes(imagePath, imageBytes);
                            extractedImagePaths.Add(filename);
                        }

                    }
                }
            }

            return extractedImagePaths;
        }

        // ...

        private class ImageRenderListener : IEventListener
        {
            public List<byte[]> Images { get; } = new List<byte[]>();

            public void EventOccurred(IEventData data, EventType type)
            {
                if (type == EventType.RENDER_IMAGE)
                {
                    var renderInfo = (ImageRenderInfo)data;
                    var imageBytes = renderInfo.GetImage().GetImageBytes();

                    Images.Add(imageBytes);
                }
            }

            public ICollection<EventType> GetSupportedEvents()
            {
                return new HashSet<EventType> { EventType.RENDER_IMAGE };
            }
        }
    }
}

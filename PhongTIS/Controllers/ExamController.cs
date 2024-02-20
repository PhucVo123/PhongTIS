using IronPdf;
using PhongTIS.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace PhongTIS.Controllers
{
    public class ExamController : Controller
    {
        PhongTISDB db = new PhongTISDB();
        string metaExercise = "";
        // GET: Exam
        public ActionResult Index()
        {
            int id = Convert.ToInt32(Session["IDSubject"]);
            var v = from t in db.Categories
                    where t.id == id
                    select t;
            return View(v.FirstOrDefault());
        }
        public ActionResult getPart(string meta)
        {
            var v = (from t in db.Exercises
                     where t.Category.meta == meta
                     group t by new { t.part } into g
                     select g.FirstOrDefault());
            return PartialView(v.ToList());
        }
        public ActionResult getTest(string part, int id)
        {

            var v = from t in db.Exercises
                    where t.hide == true && t.part == part && t.CategoryID == id
                    select t;
            return PartialView(v.ToList());

        }
        public ActionResult DetailExam(long id)
        {

            var v = from t in db.Questions
                    where t.ExerciseID == id && t.hide == true
                    orderby t.order ascending
                    select t;
            return View(v.FirstOrDefault());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult DetailExam(long id, [Bind(Include = "id,IDName,scoreTest,IDTest,meta,hide,order,datebegin")] ScoreStudent scoreOfStudent)
        {
            var v1 = from t in db.Questions
                    where t.ExerciseID == id && t.hide == true
                    orderby t.order ascending
                    select t.correct_answer;
            var score = 0;
            List<string> answer = new List<string>();
            for (var i = 1; i <= v1.ToArray().Length; i++)
            {
                answer.Add(Request.Form["answer_" + i]);
            }
            List<int> errorAnswer = new List<int>();
            for (var i = 0; i < v1.ToArray().Length; i++)
            {
                if (v1.ToArray()[i].Equals(answer[i]))
                {
                    score += 1;
                }
                else
                {
                    errorAnswer.Add(i + 1);
                }

            }
            Session["Score"] = score;
            Session["SumAnswer"] = v1.ToArray().Length;
            Session["IDTest"] = id;
            Session["Answer"] = answer.ToArray();
            Session["ErrorAnswer"] = errorAnswer.ToArray();
            scoreOfStudent.id = getMaxIdScore();
            scoreOfStudent.scoreTest = score.ToString();
            scoreOfStudent.IDTest = (int)id;
            scoreOfStudent.IDName = Convert.ToInt32(Session["IDUser"]);
            scoreOfStudent.hide = true;
            scoreOfStudent.order = getMaxIdScore() + 1;
            scoreOfStudent.datebegin = DateTime.Now;
            db.ScoreStudents.Add(scoreOfStudent);
            db.SaveChanges();
            return RedirectToAction("Result");
        }
        public ActionResult getQuestion(long id, int page = 1, int pageSize = 3)
        {
            var v = db.Questions.Where(x => x.ExerciseID == id && x.hide == true).OrderBy(x => x.order).Skip(pageSize * (page - 1)).Take(pageSize).ToList();
            var totalRecord = db.Questions.Where(x => x.ExerciseID == id && x.hide == true).Count();
            Session["totalRecord"] = totalRecord;
            var question = from t in db.Questions
                           where t.ExerciseID == id && t.hide == true
                           orderby t.order ascending
                           select t;
            ViewBag.metaExercise = question.FirstOrDefault().Exercise.meta;
            ViewBag.idExercise = question.FirstOrDefault().Exercise.id;
            ViewBag.TotalRecord = totalRecord;
            ViewBag.Page = page;
            int maxPage = 5;
            int totalPage = 0;
            totalPage = (int) Math.Ceiling((double)totalRecord / (double)pageSize);
            ViewBag.TotalPage = totalPage;
            ViewBag.MaxPage = maxPage;
            ViewBag.First = 1;
            ViewBag.Last = maxPage;
            ViewBag.Next = page + 1;
            ViewBag.Prev = page - 1;
            //var v = from t in db.Questions
            //        where t.ExerciseID == id && t.hide == true
            //        orderby t.order ascending
                    //select t;
            return PartialView(v);
        }

        public ActionResult getExam(long id)
        {
            var v = from t in db.Questions
                    where t.ExerciseID == id
                    orderby t.order ascending
                    select t;
            return PartialView(v.ToList());
        }
        public ActionResult Result()
        {
            return View();
        }
        public ActionResult ResultDetail()
        {
            var id = Session["IDTest"];
            var v = from t in db.Questions.AsEnumerable()
                     where t.ExerciseID == Int32.Parse(id.ToString()) && t.hide == true
                     orderby t.order ascending
                     select t;
            return View(v.ToList());
        }
        public int getMaxIdScore()
        {
            return db.ScoreStudents.Count();
        }

    }
}
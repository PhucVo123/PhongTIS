using PhongTIS.Models;
using System;
using System.Collections;
using System.Collections.Generic;
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
        public ActionResult DetailExam(AnswerUser answerUser,long id, [Bind(Include = "id,IDName,scoreTest,IDTest,meta,hide,order,datebegin")] ScoreStudent scoreOfStudent)
        {
            var v = from t in db.Questions
                    where t.ExerciseID == id && t.hide == true
                    orderby t.order ascending
                    select t;
            var v1 = from t in db.Questions
                    where t.ExerciseID == id && t.hide == true
                    orderby t.order ascending
                    select t.correct_answer;
            var score = 0;

            //string[] answer = ((IEnumerable)answerUser).Cast<AnswerUser>().Select(x => x.ToString()).ToArray();
            string[] answer = { answerUser.answer_1,
                                answerUser.answer_2,
                                answerUser.answer_3,
                                answerUser.answer_4,
                                answerUser.answer_5,
                                answerUser.answer_6,
                                answerUser.answer_7,
                                answerUser.answer_8,
                                answerUser.answer_9,
                                answerUser.answer_10,
                                answerUser.answer_11,
                                answerUser.answer_12,
                                answerUser.answer_13,
                                answerUser.answer_14,
                                answerUser.answer_15,
                                answerUser.answer_16,
                                answerUser.answer_17,
                                answerUser.answer_18,
                                answerUser.answer_19,
                                answerUser.answer_20,
                                answerUser.answer_21,
                                answerUser.answer_22,
                                answerUser.answer_23,
                                answerUser.answer_24,
                                answerUser.answer_25};
            List<String> errorAnswer = new List<String>();
            for (var i = 0; i < answer.Length; i++)
            {
                if (answer[i] == null)
                {
                    break;
                }
                else 
                {
                    if (v1.ToArray()[i].Equals(answer[i]))
                    {
                        score += 1;
                    }
                    else
                    {
                        errorAnswer.Add(answer[i]);
                    }    
                }

            }
            Session["Score"] = score;
            Session["SumAnswer"] = v1.ToArray().Length;
            Session["IDTest"] = id;
            Session["Answer"] = answer;
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
        public ActionResult getQuestion(long id)
        {
            var v = from t in db.Questions
                    where t.ExerciseID == id && t.hide == true
                    orderby t.order ascending
                    select t;
            return PartialView(v.ToList());
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
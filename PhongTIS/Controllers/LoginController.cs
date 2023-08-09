using PhongTIS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PhongTIS.Areas.admin.Common;
namespace PhongTIS.Controllers
{
    public class LoginController : Controller
    {
        PhongTISDB db = new PhongTISDB();
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Authen(Student student)
        {
            var password = Encryptor.MD5Hash(student.pwd);
            var check = db.Students.Where(s => s.username == student.username && s.pwd == password).FirstOrDefault();
            if (check == null)
            {
                ModelState.AddModelError("","Tài khoản không tồn tại hoặc bạn đã nhập sai.");
                return View("Index", student);
            }
            else
            {
                Session["UserName"] = student.username;
                Session["IDUser"] = check.id;
                Session["IDSubject"] = check.idSubject;
                return RedirectToAction("Index", "Exam");
            }
        }
        public ActionResult Logout()
        {
            Session.Abandon();
            return RedirectToAction("Index", "Home");
        }
    }
}
using PhongTIS.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PhongTIS.Controllers
{
    public class HomeController : Controller
    {
        PhongTISDB db = new PhongTISDB();
        public ActionResult Index()
        {
            if (Session["UserName"] !=null)
            {
                return RedirectToAction("Index","Exam");
            }
            else
            {
                return View();
            }
        }

        public ActionResult About()
        {
            return View();
        }
        public ActionResult getAbout()
        {
            var v = from t in db.AboutUs1
                    where t.hide == true
                    orderby t.order ascending
                    select t;
            return PartialView(v.FirstOrDefault());
        }
        public ActionResult Contact()
        {   
            return View();
        }
        public ActionResult getMenu()
        {
            try
            {
                var v = from t in db.Menus
                        where t.hide == true
                        orderby t.order ascending
                        select t;
                return PartialView(v.ToList());
            }
            catch(DbEntityValidationException e)
            {
                throw e;
            }
            catch(Exception e) 
            {
                throw e;
            }
        

        }
    }
}
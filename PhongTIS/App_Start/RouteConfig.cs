using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace PhongTIS
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute("Course", "{type}",
             new { controller = "Course", action = "Index" },
             new RouteValueDictionary
             {
                   {"type","khoa-hoc-phong-tis" }
             },
             namespaces: new[] { "PhongTIS.Controllers" });

            routes.MapRoute("About", "{type}",
             new { controller = "Home", action = "About" },
             new RouteValueDictionary
             {
                   {"type","ve-chung-toi" }
             },
             namespaces: new[] { "PhongTIS.Controllers" });

            routes.MapRoute("Contact", "{type}",
             new { controller = "Home", action = "Contact" },
             new RouteValueDictionary
             {
                   {"type","lien-he" }
             },
             namespaces: new[] { "PhongTIS.Controllers" });

            routes.MapRoute("Login", "{type}",
             new { controller = "Login", action = "Index" },
             new RouteValueDictionary
             {
                   {"type","dang-nhap" }
             },
             namespaces: new[] { "PhongTIS.Controllers" });

            routes.MapRoute("IndexExam", "{type}",
                new { controller = "Exam", action = "Index", meta = UrlParameter.Optional },
                new RouteValueDictionary
                {
                    {"type","bai-kiem-tra" }
                },
                namespaces: new[] { "PhongTIS.Controllers" });

            routes.MapRoute("DetailExam", "{type}/{meta}/{meta_detail}/{id}",
                new { controller = "Exam", action = "DetailExam", meta = UrlParameter.Optional },
                new RouteValueDictionary
                {
                    {"type","bai-kiem-tra" }
                },
                namespaces: new[] { "PhongTIS.Controllers" });

            routes.MapRoute("ResultDetail", "{type}",
                new { controller = "Exam", action = "ResultDetail", meta = UrlParameter.Optional },
                new RouteValueDictionary
                {
                    {"type","result-detail" }
                },
                namespaces: new[] { "PhongTIS.Controllers" });

            routes.MapRoute("PDF", "{type}/{meta}/SplitPdf",
                new { controller = "Exercises", action = "SplitPdf", meta = UrlParameter.Optional },
                new RouteValueDictionary
                {
                    {"type","admin" }
                },
                namespaces: new[] { "PhongTIS.Areas.admin.Controllers" });


            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "PhongTIS.Controllers" }
            );
        }
    }
}

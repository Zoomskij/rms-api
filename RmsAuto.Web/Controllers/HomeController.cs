using RMSAutoAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace RMSAutoAPI.Controllers
{

    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var url = WebConfigurationManager.AppSettings["UrlApi"];
            return View("~/Views/Home/Index.cshtml",null, url);
        }

        


        public ActionResult Index2()
        {
            var methods = new List<ApiMethod>();
            methods.Add(new ApiMethod() { Type = "GET", Name = "Ge tBrands", Uri = "/api/articles/{article}/brands", Group = "Articles" });
            methods.Add(new ApiMethod() { Type = "GET", Name = "Get Spare Parts", Uri = "/api/articles/{article}/brand/{brand}", Group = "Articles" });
            methods.Add(new ApiMethod() { Type = "GET", Name = "Get Partners", Uri = "/api/Partners", Group = "Partners" });

            return View(methods);
        }

        [ChildActionOnly]
        public ActionResult GetHtmlPage(string path)
        {
            return new FilePathResult(path, "text/html");
        }


        [HttpGet]
        public PartialViewResult Authorization()
        {
            return PartialView();
        }
    }
}
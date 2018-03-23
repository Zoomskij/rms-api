using RMSAutoAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
            ViewBag.CurrentUser = "Вы не авторизованы";
            //if (User.Identity.IsAuthenticated)
            //{
            //    var init = User.Identity.Name;
              
            //}
            var claims = (ClaimsIdentity)User.Identity;
            ViewBag.CurrentUser = "Ваш логин: " + TempData["Email"];
            ViewBag.Token = TempData["bearerToken"];

            //return result;

            var methods = new List<ApiMethod>();
            methods.Add(new ApiMethod() { Type = "GET", Name = "Get_Brands", Uri = "/api/articles/{article}/brands", Group = "Articles" });
            methods.Add(new ApiMethod() { Type = "GET", Name = "Get_Spare_Parts", Uri = "/api/articles/{article}/brand/{brand}", Group = "Articles" });
            methods.Add(new ApiMethod() { Type = "GET", Name = "Get_Partners", Uri = "/api/Partners", Group = "Partners" });


            methods[0].Description = "Возвращает список брендов по артикулу";
            methods[0].TitleDescription = "Возвращает только те бренды по которым имеются в наличие детали (на складе или у транзитных поставщиков). Для авторизации в HTTP-заголовок необходимо передавать пару key = “Authorization” value = “Bearer %ВАШ АВТОРИЗАЦИОННЫЙ ТОКЕН%”";
            methods[0].Parameters.Add(new ApiParameter() { Name = "article", Description = "Артикул (номер запчасти)", isRequired = true, Type = "string" });
            methods[0].Parameters.Add(new ApiParameter() { Name = "analogues", Description = "Искать аналоги. False - поиск без аналогов (значение по умолчанию). True - поиск с аналогами.", isRequired = false, Type = "boolean" });

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
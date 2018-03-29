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
        public static string Token { get; set; }
        public static string CurrentEmail { get; set; }

        public ActionResult Index()
        {
            if (!string.IsNullOrWhiteSpace((string)TempData["bearerToken"]) && !TempData["bearerToken"].ToString().Equals(Token))
            {
                Token = (string)TempData["bearerToken"];
                CurrentEmail = (string)TempData["Username"];
            }
            if ((int?)TempData["logout"] == 1)
            {
                Token = string.Empty;
                CurrentEmail = string.Empty;
            }

            ViewBag.CurrentUser = CurrentEmail;
            ViewBag.Token = Token;

            if (!string.IsNullOrWhiteSpace(ViewBag.Token))
                Token = ViewBag.Token;
            //return result;

            var methods = new List<ApiMethod>();
            methods.Add(new ApiMethod() { Type = "GET", Name = "GetBrands", Uri = "/api/articles/{article}/brands", Group = "Articles", });
            methods.Add(new ApiMethod() { Type = "GET", Name = "GetSpareParts", Uri = "/api/articles/{article}/brand/{brand}", Group = "Articles" });
            methods.Add(new ApiMethod() { Type = "GET", Name = "GetPartners", Uri = "/api/partners", Group = "Partners" });


            methods[0].Response = new Brand();
            methods[0].Description = "Возвращает список брендов по артикулу";
            methods[0].TitleDescription = "Возвращает только те бренды по которым имеются в наличие детали (на складе или у транзитных поставщиков). Для авторизации в HTTP-заголовок необходимо передавать пару key = “Authorization” value = “Bearer %ВАШ АВТОРИЗАЦИОННЫЙ ТОКЕН%”";
            methods[0].Parameters.Add(new ApiParameter() { Name = "article", Description = "Артикул (номер запчасти)", IsRequired = true, Type = "string", TypeParameter = TypeParameter.path });
            methods[0].Parameters.Add(new ApiParameter() { Name = "analogues", Description = "Искать аналоги. False - поиск без аналогов (значение по умолчанию). True - поиск с аналогами.", IsRequired = false, Type = "boolean", TypeParameter = TypeParameter.query });

            ////////////
            methods[1].Response = new PartNumber();
            methods[1].Description = "Возвращает список запчастей";
            methods[1].TitleDescription = "Для авторизации в HTTP-заголовок необходимо передавать пару key = “Authorization” value = “Bearer %ВАШ АВТОРИЗАЦИОННЫЙ ТОКЕН%”";
            methods[1].Parameters.Add(new ApiParameter() { Name = "article", Description = "Артикул (номер запчасти)", IsRequired = true, Type = "string", TypeParameter = TypeParameter.path });
            methods[1].Parameters.Add(new ApiParameter() { Name = "brand", Description = "Бренд", IsRequired = true, Type = "string", TypeParameter = TypeParameter.path });
            methods[1].Parameters.Add(new ApiParameter() { Name = "analogues", Description = "Искать аналоги. False - поиск без аналогов (значение по умолчанию). True - поиск с аналогами.", IsRequired = false, Type = "boolean", TypeParameter = TypeParameter.query });
            ////////
            methods[2].Response = new Partner();
            methods[2].Description = "Возвращает список партнеров";
            methods[2].TitleDescription = "Для того чтобы авторизоваться в другом регионе необходим параметр InternalFranchName";

            return View(methods);
        }
    }
}
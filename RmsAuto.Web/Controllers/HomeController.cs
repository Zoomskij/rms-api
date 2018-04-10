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
        public static string CurrentUser { get; set; }

        public ActionResult Index()
        {
            if (!string.IsNullOrWhiteSpace((string)TempData["bearerToken"]) && !TempData["bearerToken"].ToString().Equals(Token))
            {
                Token = (string)TempData["bearerToken"];
				CurrentUser = (string)TempData["Username"];
            }
            if ((int?)TempData["logout"] == 1)
            {
                Token = string.Empty;
				CurrentUser = string.Empty;
            }

            ViewBag.CurrentUser = CurrentUser;
            ViewBag.Token = Token;

            if (!string.IsNullOrWhiteSpace(ViewBag.Token))
                Token = ViewBag.Token;
            //return result;

            var methods = new List<ApiMethod>();
            methods.Add(new ApiMethod() { Type = "GET", Name = "GetBrands", Uri = "/api/articles/{article}/brands", Group = "Articles", });
            methods.Add(new ApiMethod() { Type = "GET", Name = "GetSpareParts", Uri = "/api/articles/{article}/brand/{brand}", Group = "Articles" });
            methods.Add(new ApiMethod() { Type = "GET", Name = "GetOrders", Uri = "/api/orders", Group = "Orders" });
            methods.Add(new ApiMethod() { Type = "GET", Name = "GetOrder", Uri = "/api/orders/{orderId}", Group = "Orders" });
            methods.Add(new ApiMethod() { Type = "POST", Name = "CreateOrder", Uri = "/api/orders", Group = "Orders" });
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


            methods[2].Response = new List<Order<PartNumber>>();
            methods[2].Description = "Возвращает все заказы текущего пользователя";
            methods[2].TitleDescription = "Для авторизации в HTTP-заголовок необходимо передавать пару key = “Authorization” value = “Bearer %ВАШ АВТОРИЗАЦИОННЫЙ ТОКЕН%”";
            /////
            methods[3].Response = new Order<PartNumber>();
            methods[3].Description = "Возвращает информацию по заказу";
            methods[3].TitleDescription = "Для авторизации в HTTP-заголовок необходимо передавать пару key = “Authorization” value = “Bearer %ВАШ АВТОРИЗАЦИОННЫЙ ТОКЕН%”";
            methods[3].Parameters.Add(new ApiParameter() { Name = "orderId", Description = "ID заказа", IsRequired = true, Type = "int", TypeParameter = TypeParameter.path });

            methods[4].Response = new Order<PartNumber>();
            methods[4].Description = "Позволяет создать заказ";
            methods[4].TitleDescription = "Для авторизации в HTTP-заголовок необходимо передавать пару key = “Authorization” value = “Bearer %ВАШ АВТОРИЗАЦИОННЫЙ ТОКЕН%”";
            methods[4].Parameters.Add(new ApiParameter() { Name = "orders", Description = "Заказ", IsRequired = true, Type = "object", TypeParameter = TypeParameter.body });

            ////////
            methods[5].Response = new Partner();
            methods[5].Description = "Возвращает список партнёров";
            methods[5].TitleDescription = "Для авторизации клиентов наших региональных партнёров необходимо передавать код партнёра, полученный в этом методе (параметр Code)";


            ViewBag.OrderModel = new Order<OrderPartNumbers>
            {
                OrderName = "Новый Заказ",
                Reaction = Reaction.AnyPush,
                PartNumbers = new List<OrderPartNumbers>()
                 {
                      new OrderPartNumbers()
                      {
                            Article = "333310",
                            Brand = "KAYABA",
                            SupplierID = 21,
                            Count = 2,
                            Price = Convert.ToDecimal(1000.25),
                            ReacionByChange = 0,
                            ReactionByCount = 0,
                            ReactionByPrice = 0,
                            ReactionBySupplier = 0
                      }
                 }
            };
            return View(methods);
        }
    }
}
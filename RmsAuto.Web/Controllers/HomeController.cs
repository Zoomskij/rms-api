using AutoMapper;
using RMSAutoAPI.App_Data;
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

            using (var db = new ex_rmsauto_storeEntities())
            {
               // db.Configuration.LazyLoadingEnabled = false;
                var getMethods = db.Methods.ToList();
                var methods = Mapper.Map<List<Methods>, List<ApiMethod>>(getMethods);
                foreach (var method in methods)
                {
                    switch (method.Name)
                    {
                        case "GetBrands": method.Response = new Brand(); break;
                        case "GetSpareParts": method.Response = new PartNumber(); break;
                        case "GetOrders": method.Response = new List<Order<PartNumber>>(); break;
                        case "GetOrder": method.Response = new Order<PartNumber>(); break;
                        case "CreateOrder": method.Response = new Order<ResponsePartNumbers>(); break;
                        case "GetPartners": method.Response = new Partner(); break;
                    }
                }

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
}
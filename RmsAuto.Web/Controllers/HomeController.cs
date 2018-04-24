using AutoMapper;
using RMSAutoAPI.App_Data;
using RMSAutoAPI.Helpers;
using RMSAutoAPI.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace RMSAutoAPI.Controllers
{
    public class HomeController : Controller
    {
        public string Token
        {
            get => Session["bearerToken"]?.ToString();
        }
        public string CurrentUser
        {
            get => Session["Username"]?.ToString();
        }

        [AllowAnonymous]
        public ActionResult Index()
        {
            var a = User.Identity;
            if (!string.IsNullOrWhiteSpace((string)TempData["bearerToken"]) && !TempData["bearerToken"].ToString().Equals(Token))
            {
                Session["bearerToken"] = (string)TempData["bearerToken"];
                Session["Username"] = (string)TempData["Username"];
            }

            if ((int?)TempData["logout"] == 1)
            {
                Session["bearerToken"] = null;
                Session["Username"] = null;
                TempData["logout"] = null;
            }

            ViewBag.CurrentUser = CurrentUser;
            ViewBag.Token = Token ?? string.Empty;

            using (var db = new ex_rmsauto_storeEntities())
            {
                var getMethods = db.Methods.Where(x => x.Visible == true).ToList();
                var methods = Mapper.Map<List<Methods>, List<ApiMethod>>(getMethods);
                foreach (var method in methods)
                {
                    switch (method.Name)
                    {
                        case "GetBrands": method.Response = new Brand(); break;
                        case "GetSpareParts": method.Response = new SparePart(); break;
                        case "GetOrders": method.Response = new List<Order>(); break;
                        case "GetOrder": method.Response = new Order(); break;
                        case "CreateOrder": method.Response = new OrderPlaced(); break;
                        case "GetPartners": method.Response = new Partner(); break;
                    }
                }

                ViewBag.Models = ModelHelper.InitModels();
                ViewBag.OrderModel = ModelHelper.InitOrder();

                return View(methods);
            }
        }
    }
}
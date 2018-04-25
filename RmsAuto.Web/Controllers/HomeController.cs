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
        private string _token;
        private string _userName;
        public string Token
        {
            get => Session["Token"]?.ToString();
            set
            {
                _token = value;
                Session["Token"] = value;
            }
        }
        public string UserName
        {
            get => Session["UserName"]?.ToString();
            set
            {
                _userName = value;
                Session["UserName"] = value;
            }
        }

        [AllowAnonymous]
        public ActionResult Index()
        {
            if (!string.IsNullOrWhiteSpace((string)TempData["Token"]) && !TempData["Token"].ToString().Equals(Token))
            {
                Token = (string)TempData["Token"];
                UserName = (string)TempData["UserName"];
            }

            if ((int?)TempData["logout"] == 1)
            {
                Token = null;
                UserName = null;
                TempData["logout"] = null;
            }

            ViewBag.UserName = UserName;
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

                var UserPermissions = new List<int>();
                var userPermissions = db.Users.FirstOrDefault(x => x.Username == UserName)?.Permissions;
                if (userPermissions != null)
                {
                    foreach (var up in userPermissions)
                        UserPermissions.Add(up.ID);
                    ViewBag.UserPermissions = UserPermissions;
                }
                else ViewBag.UserPermissions = new List<int>();

                return View(methods);
            }
        }
    }
}
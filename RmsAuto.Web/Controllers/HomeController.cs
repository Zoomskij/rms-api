using AutoMapper;
using RMSAutoAPI.App_Data;
using RMSAutoAPI.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
                TempData["bearerToken"] = null;
                TempData["Username"] = null;
                Token = string.Empty;
				CurrentUser = string.Empty;
            }

            ViewBag.CurrentUser = CurrentUser;
            ViewBag.Token = Token ?? string.Empty;

            if (!string.IsNullOrWhiteSpace(ViewBag.Token))
                Token = ViewBag.Token;
            //return result;

            using (var db = new ex_rmsauto_storeEntities())
            {
               // db.Configuration.LazyLoadingEnabled = false;
                var getMethods = db.Methods.Where(x => x.Visible == true).ToList();
                var methods = Mapper.Map<List<Methods>, List<ApiMethod>>(getMethods);
                foreach (var method in methods)
                {
                    switch (method.Name)
                    {
                        case "GetBrands": method.Response = new Brand(); break;
                        case "GetSpareParts": method.Response = new SparePart(); break;
                        case "GetOrders": method.Response = new List<Order<SparePart>>(); break;
                        case "GetOrder": method.Response = new Order<SparePart>(); break;
                        case "CreateOrder": method.Response = new Order<SparePart>(); break;
                        case "GetPartners": method.Response = new Partner(); break;
                    }
                }

                List<object> objModels = new List<object>();
                objModels.Add(new Brand());
                objModels.Add(new SparePart());
                objModels.Add(new Order<SparePart>());
                objModels.Add(new ResponseSparePart());
                objModels.Add(new Partner());

                var models = new List<Model>();
                foreach (var response in objModels)
                {
                    var model = new Model();
                    model.Name = response.GetType().Name.Replace("`1", string.Empty);
                    foreach (var property in response.GetType().GetProperties())
                    {
                        var parameter = new Parameter();
                        parameter.Name = property.Name;
                        switch (property.PropertyType.Name.ToLower())
                        {
                            case "nullable`1":
                                parameter.Type = "int 32 (nullable)";
                                break;
                            case "list`1":
                                parameter.Type = "array[]";
                                break;
                            case "sparepartitemtype":
                                parameter.Type = "int32";
                                break;
                            case "quality":
                                parameter.Type = "int32";
                                break;
                            case "reaction":
                                parameter.Type = "int32";
                                break;
                            case "statussparepart":
                                parameter.Type = "int32";
                                break;
                            default:
                                parameter.Type = property.PropertyType.Name.ToLower();
                                break;
                        }
                        parameter.Description = ((DescriptionAttribute)property.GetCustomAttributes(typeof(DescriptionAttribute), true).FirstOrDefault())?.Description;
                        model.Parameters.Add(parameter);
                    }
                    models.Add(model);
                }
                ViewBag.Models = models;

                ViewBag.OrderModel = new Order<OrderSpareParts>
                {
                    OrderName = "Новый Заказ",
                    Reaction = Reaction.AnyPush,
                    SpareParts = new List<OrderSpareParts>()
                     {
                          new OrderSpareParts()
                          {
                                Article = "333310",
                                Brand = "KAYABA",
                                SupplierID = 21,
                                Count = 2,
                                Price = Convert.ToDecimal(10000.00),
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
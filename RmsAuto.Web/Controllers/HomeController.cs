using AutoMapper;
using RMSAutoAPI.App_Data;
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

                List<object> objModels = new List<object>();
                objModels.Add(new Brand());
                objModels.Add(new SparePart());
                objModels.Add(new Order());
                objModels.Add(new OrderLine());
                objModels.Add(new OrderHead());
                objModels.Add(new OrderHeadLine());
                objModels.Add(new OrderPlaced());
                objModels.Add(new OrderPlacedLine());
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
                        var isRequired = ((RequiredAttribute)property.GetCustomAttributes(typeof(RequiredAttribute), true).FirstOrDefault());
                        parameter.IsRequired = isRequired == null ? false : true;
                        model.Parameters.Add(parameter);
                    }
                    models.Add(model);
                }
                ViewBag.Models = models;

                ViewBag.OrderModel = new OrderHead
                {
                    CustOrderNum = "1234",
                    OrderNotes = "Комментарий к заказу",
                    ValidationType = Reaction.AnyPush,
                    OrderHeadLines = new List<OrderHeadLine>()
                     {
                          new OrderHeadLine()
                          {
                                Article = "333310",
                                Brand = "KAYABA",
                                SupplierID = 21,
                                Count = 2,
                                Price = Convert.ToDecimal(3000.00),
                                StrictlyThisNumber = false,
                                ReactionByCount = 0,
                                ReactionByPrice = 0
                          },
                          new OrderHeadLine()
                          {
                              Article = "555132E100",
                              Brand = "MOBIS",
                              SupplierID = 1203,
                              Count = 1,
                              Price = Convert.ToDecimal(120.00),
                              StrictlyThisNumber = false,
                              ReactionByCount = 0,
                              ReactionByPrice = 0
                          }

                     }
                };
                return View(methods);
            }
        }
    }
}
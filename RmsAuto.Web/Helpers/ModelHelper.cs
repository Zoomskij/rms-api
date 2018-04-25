using RMSAutoAPI.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RMSAutoAPI.Helpers
{
    public static class ModelHelper
    {
        /// <summary>
        /// Данные модели будут выведены снизу в описании. Также преобразует нечитаемые типы вроде nullable'1 к int 32 (nullable)
        /// </summary>
        /// <param name="objModels"></param>
        /// <returns></returns>
        public static List<Model> InitModels()
        {
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
                        case "responsepartnumber":
                            parameter.Type = "int32";
                            break;
                        case "nullable`1":
                            parameter.Type = "int32 (nullable)";
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
            return models;
        }

        /// <summary>
        /// Устанавливает стандартный шаблон для отправки заказа
        /// </summary>
        /// <returns></returns>
        public static OrderHead InitOrder()
        {
            var orderHead = new OrderHead
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
            return orderHead;
        }
    }
}
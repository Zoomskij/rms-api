﻿using AutoMapper;
using Newtonsoft.Json;
using RMSAutoAPI.App_Data;
using RMSAutoAPI.Helpers;
using RMSAutoAPI.Infrastructure;
using RMSAutoAPI.Models;
using RMSAutoAPI.Properties;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Web.Http;

namespace RMSAutoAPI.Controllers
{
    [RoutePrefix("api")]
    public class OrdersController : ApiController
    {
        private ex_rmsauto_storeEntities db = new ex_rmsauto_storeEntities();


        public static Users CurrentUser { get; set; }
        private Orders _dbOrder;
        public Orders DbOrder
        {
            get { return _dbOrder; }
            set
            {
                _dbOrder = value;
                _dbOrder.UserID = CurrentUser.UserID;
                _dbOrder.ClientID = CurrentUser.AcctgID;
                _dbOrder.Users = CurrentUser;
                _dbOrder.OrderDate = DateTime.Now;
                _dbOrder.Status = 0;
                _dbOrder.PaymentMethod = (byte)PaymentMethod.Cash;
                _dbOrder.ShippingMethod = 0;
                _dbOrder.StoreNumber = "StoreNumber";
            }
        }

        public OrdersController()
        {
            var claims = (ClaimsIdentity)User.Identity;
            var region = claims.Claims.FirstOrDefault(x => x.Type.Equals("Region"))?.Value;

            if (!string.IsNullOrWhiteSpace(region))
            {
                var currentFranch = db.spGetFranches().FirstOrDefault(x => x.InternalFranchName.ToUpper().Equals(region.ToUpper()));
                db.ChangeDatabase(initialCatalog: $"ex_{currentFranch.DbName}_store", dataSource: $"{currentFranch.ServerName}");
            }
            else db = new ex_rmsauto_storeEntities();

            var userName = User.Identity.Name;
            CurrentUser = db.Users.FirstOrDefault(x => x.Username == userName);
        }

        [HttpGet]
        [Route("orders")]
        [Authorize]
        //[Authorize(Roles = "Client_SearchApi, NoAccess")]
        public IHttpActionResult GetOrders()
        {
            var orders = db.Orders.Where(x => x.UserID == CurrentUser.UserID);
            if (!orders.Any()) return Ok(new List<Orders>());
            List<Order<SparePart>> userOrders = new List<Order<SparePart>>();

            foreach (var order in orders)
            {
                var newOrder = Mapper.Map<Orders, Order<SparePart>>(order);

                userOrders.Add(newOrder);
            }
            return Ok(userOrders);
        }

        [HttpGet]
        [Route("orders/{orderId}")]
        [Authorize]
        //[Authorize(Roles = "Client_SearchApi, NoAccess")]
        public IHttpActionResult GetOrder(int orderId)
        {
            var userName = User.Identity.Name;

            var order = db.Orders.FirstOrDefault(x => x.OrderID == orderId);
            if (order == null) return Ok(new Orders());
            var userOrder = Mapper.Map<Orders, Order<SparePart>>(order);

            var orderLines = Mapper.Map<ICollection<OrderLines>, List<SparePart>>(order.OrderLines);

            return Ok(userOrder);
        }

        [HttpPost]
        [Route("orders")]
        [Authorize]
        public IHttpActionResult CreateOrder([FromBody] Order<OrderSparePart> order)
        {
            DbOrder = new Orders();
            var respOrder = new Order<ResponseSparePart>();
            var parts = new List<spGetSparePart_Result>();
            using (var dbTransaction = db.Database.BeginTransaction())
            {
                var orderLineStatus = db.OrderLineStatuses.FirstOrDefault(x => x.OrderLineStatusID == 10);

                // TO DO: Заменить на хранимку, которая будет извлекать все детали разом
                foreach (var sparePart in order.SpareParts)
                {
                    var part = db.spGetSparePart(sparePart.Brand, sparePart.Article, sparePart.SupplierID, CurrentUser.AcctgID).FirstOrDefault();
                    parts.Add(part);
                }

                foreach (var sparePart in order.SpareParts)
                {
                    var dbOrderLine = new OrderLines();
                    var respOrderLine = new ResponseSparePart();
                    respOrderLine.PriceOrder = sparePart.Price;
                    respOrderLine.CountOrder = sparePart.Count;

                    //Обрабатываем ошибки
                    if (string.IsNullOrWhiteSpace(sparePart.Article)  || string.IsNullOrWhiteSpace(sparePart.Brand) || sparePart.SupplierID == 0)
                    {
                        respOrderLine.Status = ResponsePartNumber.ErrorKey;
                        respOrder.SpareParts.Add(respOrderLine);
                        continue;
                    }
                    if (sparePart.Count <= 0)
                    {
                        respOrderLine.Status = ResponsePartNumber.CountNotSet;
                        respOrder.SpareParts.Add(respOrderLine);
                        continue;
                    }
                    //Проверяем коллизию по количеству
                    if (order.Reaction == Reaction.CheckRow && ((sparePart.ReactionByCount < 0) || (sparePart.ReactionByCount > 3)))
                    {
                        respOrderLine.Status = ResponsePartNumber.NotSetReactionByCount;
                        respOrder.SpareParts.Add(respOrderLine);
                        continue;
                    }
                    if (order.Reaction == Reaction.CheckRow && ((sparePart.ReactionByPrice < 0) || (sparePart.ReactionByPrice > 1)))
                    {
                        respOrderLine.Status = ResponsePartNumber.NotSetReactionByPrice;
                        respOrder.SpareParts.Add(respOrderLine);
                        continue;
                    }

                    var part = parts.FirstOrDefault(x => x.Manufacturer == sparePart.Brand && x.PartNumber == sparePart.Article && x.SupplierID == sparePart.SupplierID);
                    switch (order.Reaction)
                    {
                        case Reaction.NotErrorPush:
                            if (sparePart.Count > part.QtyInStock)
                            {
                                respOrderLine.Status = ResponsePartNumber.ErrorCount;
                                break;
                            }
                            if (sparePart.Price < part.FinalPrice)
                            {
                                respOrderLine.Status = ResponsePartNumber.WrongPrice;
                                respOrder.SpareParts.Add(respOrderLine);
                                continue;
                            }
                            respOrderLine.PriceApproved = sparePart.Price;
                            respOrderLine.CountApproved = sparePart.Count;
                            break;
                        case Reaction.AnyPush:
                            respOrderLine.PriceOrder = sparePart.Price;
                            respOrderLine.CountApproved = dbOrderLine.Qty = GetMoreMinQty(sparePart.Count, part.MinOrderQty, part.QtyInStock.Value);
                            respOrderLine.PriceApproved = dbOrderLine.UnitPrice = Math.Round(part.FinalPrice.Value, 2);
                            break;
                        case Reaction.CheckRow:
                            switch (sparePart.ReactionByCount)
                            {
                                // берем только указанное количество
                                case 0:
                                    if (sparePart.Count > part.QtyInStock)
                                    {
                                        respOrderLine.CountApproved = 0;
                                        respOrderLine.Status = ResponsePartNumber.ErrorCount;
                                    }
                                    else
                                    {
                                        dbOrderLine.Qty = respOrderLine.CountApproved = respOrderLine.CountOrder = sparePart.Count;
                                    }
                                    break;
                                // Берем сколько есть, но не выше указанного
                                case 1:
                                    if (sparePart.Count > part.QtyInStock)
                                    {
                                        dbOrderLine.Qty = respOrderLine.CountApproved = part.QtyInStock.Value;
                                    }
                                    else
                                    {
                                        dbOrderLine.Qty = respOrderLine.CountApproved = sparePart.Count;
                                    }
                                    break;
                                // Разрешаем выравнивать вверх по MinQty
                                case 2:
                                    dbOrderLine.Qty = respOrderLine.CountApproved = GetMoreMinQty(sparePart.Count, part.MinOrderQty.Value, part.QtyInStock.Value);
                                    respOrderLine.Status = ResponsePartNumber.OkCountMore;
                                    break;
                                // Разрешаем выравнивать вниз по MinQty
                                case 3:
                                    dbOrderLine.Qty = respOrderLine.CountApproved = GetLessMinQty(sparePart.Count, part.MinOrderQty.Value, part.QtyInStock.Value);
                                    respOrderLine.Status = ResponsePartNumber.OkCountLess;
                                    break;
                            }

                            switch (sparePart.ReactionByPrice)
                            {
                                // Не выше указанной цены
                                case 0:
                                    if (sparePart.Price < part.FinalPrice)
                                    {
                                        respOrderLine.Status = ResponsePartNumber.WrongPrice;
                                        respOrder.SpareParts.Add(respOrderLine);
                                        continue;
                                    }
                                    else
                                    {
                                        dbOrderLine.UnitPrice = respOrderLine.PriceApproved = Math.Round(part.FinalPrice.Value, 2);
                                    }
                                    break;
                                // текущая цена поставщика (без проверки)
                                case 1:
                                    dbOrderLine.UnitPrice = respOrderLine.PriceApproved = Math.Round(part.FinalPrice.Value, 2);
                                    break;
                            }
                            break;
                    }

                    dbOrderLine.PartNumber = respOrderLine.Article = part.PartNumber;
                    dbOrderLine.Manufacturer = respOrderLine.Brand = part.Manufacturer;
                    dbOrderLine.SupplierID = respOrderLine.SupplierID = part.SupplierID;
                    dbOrderLine.DeliveryDaysMin = sparePart != null ? part.DeliveryDaysMin : 0;
                    dbOrderLine.DeliveryDaysMax = sparePart != null ? part.DeliveryDaysMax ?? 0 : 0;
                    dbOrderLine.PartName = sparePart != null ? part.PartName : string.Empty;
                    dbOrderLine.UnitPrice = sparePart != null ? Math.Round(part.FinalPrice.Value, 2) : 0;
                    dbOrderLine.StrictlyThisNumber = sparePart.StrictlyThisNumber;
                    dbOrderLine.CurrentStatus = 0;
                    dbOrderLine.Processed = 0;
                    dbOrderLine.OrderLineStatuses = orderLineStatus;
                    DbOrder.Total +=  Math.Round(part.FinalPrice.Value,2) * dbOrderLine.Qty;


                    DbOrder.OrderLines.Add(dbOrderLine);
                    
                    respOrderLine.Reference = sparePart.Reference;
                    respOrder.SpareParts.Add(respOrderLine);
                }

                DbOrder.OrderNotes = respOrder.OrderName = order.OrderName;

                if (!respOrder.SpareParts.Any())
                {
                    respOrder.Status = 2;  // заказ не размещен
                }
                if (!respOrder.SpareParts.Any(x => x.Status != 0))
                {
                    respOrder.Status = 1; // заказ размещен частично
                }

                try
                {
                    respOrder.Total = DbOrder.Total;
                    respOrder.Username = CurrentUser.Username;
                    var createorder = db.Orders.Add(DbOrder);
                    if (DbOrder.OrderLines.Any())
                    {
                        if (respOrder.SpareParts.Any(x => x.Status != ResponsePartNumber.Ok))
                        {
                            respOrder.Status = 3;
                        }
                        db.SaveChanges();
                        if (DbOrder.OrderID != 0)
                        {
                            var orderHelper = new OrderHelper(db);
                            orderHelper.SendOrder(DbOrder, string.Empty);
                            dbTransaction.Commit();

                            respOrder.OrderId = DbOrder.OrderID;
                            respOrder.OrderDate = DbOrder.OrderDate;
                            respOrder.Status = 0;
                            return Ok(respOrder);
                        }
                    }
                    else
                    {
                        respOrder.Status = 1;
                    }
                }
                catch (DbEntityValidationException e)
                {
                    dbTransaction.Rollback();
                }


            }
            return Ok(respOrder);
        }

        public int GetMoreMinQty(int orderCount, int? minQty, int stockCount)
        {
            if (minQty.HasValue)
            {
                var value = orderCount + (orderCount % minQty.Value);
                if (value < stockCount)
                    return value;
                else
                {
                    return stockCount - (stockCount % minQty.Value);
                }
            }
            return orderCount;
        }

        public int GetLessMinQty(int orderCount, int? minQty, int stockCount)
        {
            if (minQty.HasValue)
            {
                var value = orderCount - (orderCount % minQty.Value);
                if (value < stockCount)
                    return value;
                else
                {
                    return stockCount - (stockCount % minQty.Value);
                }
            }
            return orderCount;
        }

        public static string ConvertManufacturerToSP(string manufacturer)
        {
            Regex reg = new Regex("[&><\"]");
            if (reg.IsMatch(manufacturer))
            {
                Dictionary<string, string> replaces = new Dictionary<string, string>();
                replaces.Add("&", "&amp;");
                replaces.Add("\"", "&quot;");
                replaces.Add(">", "&gt;");
                replaces.Add("<", "&lt;");

                foreach (var ch in replaces)
                    manufacturer = manufacturer.Replace(ch.Key, ch.Value);
            }
            return manufacturer;
        }
    }
}

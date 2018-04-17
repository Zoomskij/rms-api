using AutoMapper;
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
        //[Authorize(Roles = "Client_SearchApi, NoAccess")]
        public IHttpActionResult CreateOrder([FromBody] Order<OrderSpareParts> order)
        {
            using (var dbTransaction = db.Database.BeginTransaction())
            {
                var orderStatus = db.OrderLineStatuses.FirstOrDefault(x => x.OrderLineStatusID == 10);
                decimal total = 0;
                Orders dbOrder = new Orders
                {
                    UserID = CurrentUser.UserID,
                    ClientID = CurrentUser.AcctgID,
                    StoreNumber = "StoreNumber",
                    ShippingMethod = 0,
                    PaymentMethod = (byte)PaymentMethod.Cash,
                    Status = 0,
                    OrderDate = DateTime.Now,
                    Users = CurrentUser
                };


                var respOrder = Mapper.Map<Orders, Order<ResponseSparePart>>(dbOrder);
                foreach (var pn in order.SpareParts)
                {
                    var orderLine = Mapper.Map<OrderSpareParts, OrderLines>(pn);
                    var respLine = Mapper.Map<OrderLines, ResponseSparePart>(orderLine);
                    var sparePart = db.spGetSparePart(pn.Brand, pn.Article, pn.SupplierID, CurrentUser.AcctgID).FirstOrDefault();
                    if (sparePart != null)
                    {
                        switch (pn.ReactionByCount)
                        {
                            case 0:
                                if (sparePart?.QtyInStock < pn?.Count)
                                {
                                    respLine.Status = ResponsePartNumber.NotSetReactionByCount;
                                    respOrder.SpareParts.Add(respLine);
                                    continue;
                                }
                                respLine.CountApproved = pn.Count.Value;
                                break;
                            case 1:
                                if (sparePart?.QtyInStock < pn?.Count)
                                {
                                    respLine.CountApproved = orderLine.Qty = sparePart.QtyInStock.Value;
                                }
                                break;
                            case 2:
                                if (sparePart?.QtyInStock < pn?.Count && sparePart.MinOrderQty.HasValue)
                                {
                                    respLine.CountApproved = orderLine.Qty = pn.Count.Value + sparePart.MinOrderQty.Value - (pn.Count.Value % sparePart.MinOrderQty.Value);
                                }
                                break;
                        }

                        switch (pn.ReactionByPrice)
                        {
                            case 0:
                                if (sparePart?.FinalPrice > pn.Price)
                                {
                                    respLine.Status = ResponsePartNumber.WrongPrice;
                                    respOrder.SpareParts.Add(respLine);
                                    continue;
                                }
                                respLine.PriceApproved = Math.Round(sparePart.FinalPrice.Value,2);
                                break;
                            case 1:
                                if (sparePart.FinalPrice.HasValue)
                                {
                                    orderLine.UnitPrice = respLine.PriceApproved = Math.Round(sparePart.FinalPrice.Value, 2);
                                }
                                break;
                        }
                        total += sparePart.FinalPrice.Value * orderLine.Qty;

                        orderLine.DeliveryDaysMin = sparePart != null ? sparePart.DeliveryDaysMin : 0;
                        orderLine.DeliveryDaysMax = sparePart != null ? sparePart.DeliveryDaysMax ?? 0 : 0;
                        orderLine.PartName = sparePart != null ? sparePart.PartName : string.Empty;
                        orderLine.UnitPrice = sparePart != null ? Math.Round(sparePart.FinalPrice.Value,2) : 0;
                        orderLine.StrictlyThisNumber = false;
                        orderLine.CurrentStatus = 0;
                        orderLine.Processed = 0;
                        orderLine.OrderLineStatuses = orderStatus;

                        respOrder.OrderName = order.OrderName;
                        respOrder.SpareParts.Add(respLine);
                        dbOrder.OrderLines.Add(orderLine);
                    }
                    else
                    {
                        respLine.Status = ResponsePartNumber.NotFound;
                        respOrder.SpareParts.Add(respLine);
                    }
                }
                respOrder.Total = dbOrder.Total = total;

                try
                {
                    var createorder = db.Orders.Add(dbOrder);
                    var orderHelper = new OrderHelper(db);
                    
                    db.SaveChanges();
                    if (dbOrder.OrderID != 0)
                    {
                        orderHelper.SendOrder(dbOrder, string.Empty);
                        respOrder.OrderDate = dbOrder.OrderDate;
                        respOrder.OrderId = dbOrder.OrderID;
                        dbTransaction.Commit();
                        return Ok(respOrder);
                    }
                }
                catch (DbEntityValidationException e)
                {
                    dbTransaction.Rollback();
                }
            }

            return Ok(order);
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

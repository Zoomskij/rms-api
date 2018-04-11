using AutoMapper;
using Newtonsoft.Json;
using RMSAutoAPI.App_Data;
using RMSAutoAPI.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Net.Http;
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
            var userName = User.Identity.Name;
            if (string.IsNullOrWhiteSpace(userName))
            {
                CurrentUser = null;
            }
            else
            {
                if (CurrentUser == null || (CurrentUser != null && CurrentUser.Username != userName))
                    CurrentUser = db.Users.FirstOrDefault(x => x.Username == userName);
            }
        }

        [HttpGet]
        [Route("orders")]
        [Authorize]
        //[Authorize(Roles = "Client_SearchApi, NoAccess")]
        public IHttpActionResult GetOrders()
        {
            var orders = db.Orders.Where(x => x.UserID == CurrentUser.UserID);
            if (!orders.Any()) return NotFound();
            List<Order<PartNumber>> userOrders = new List<Order<PartNumber>>();

            foreach (var order in orders)
            {
                var newOrder = Mapper.Map<Orders, Order<PartNumber>>(order);

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
            if (order == null) return NotFound();
            var userOrder = Mapper.Map<Orders, Order<PartNumber>>(order);

            var orderLines = Mapper.Map<ICollection<OrderLines>, List<PartNumber>>(order.OrderLines);

            return Ok(userOrder);
        }

        [HttpPost]
        [Route("orders")]
        [Authorize]
        //[Authorize(Roles = "Client_SearchApi, NoAccess")]
        public IHttpActionResult CreateOrder([FromBody] Order<OrderPartNumbers> order)
        {
            using (var dc = new ex_rmsauto_storeEntities())
            {
                using (var dbTransaction = dc.Database.BeginTransaction())
                {
                    var orderStatus = dc.OrderLineStatuses.FirstOrDefault(x => x.OrderLineStatusID == 10);
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


                    var respOrder = Mapper.Map<Orders, Order<ResponsePartNumbers>>(dbOrder);
                    foreach (var pn in order.PartNumbers)
                    {
                        var orderLine = Mapper.Map<OrderPartNumbers, OrderLines>(pn);
                        var respLine = Mapper.Map<OrderLines, ResponsePartNumbers>(orderLine);
                        var sparePart = dc.spGetSparePart(pn.Brand, pn.Article, pn.SupplierID, CurrentUser.AcctgID).FirstOrDefault();
                        if (sparePart != null)
                        {
                            switch (pn.ReactionByCount)
                            {
                                case 0:
                                    if (sparePart?.QtyInStock < pn?.Count)
                                    {
                                        respLine.Status = ResponsePartNumber.NotSetReactionByCount;
                                        respOrder.PartNumbers.Add(respLine);
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
                                        respOrder.PartNumbers.Add(respLine);
                                        continue;
                                    }
                                    respLine.PriceApproved = sparePart.FinalPrice.Value;
                                    break;
                                case 1:
                                    if (sparePart.FinalPrice.HasValue)
                                    {
                                        orderLine.UnitPrice = respLine.PriceApproved = sparePart.FinalPrice.Value;
                                    }
                                    break;
                            }
                            total += sparePart.FinalPrice.Value * orderLine.Qty;

                            orderLine.DeliveryDaysMin = sparePart != null ? sparePart.DeliveryDaysMin : 0;
                            orderLine.DeliveryDaysMax = sparePart != null ? sparePart.DeliveryDaysMax ?? 0 : 0;
                            orderLine.PartName = sparePart != null ? sparePart.PartName : string.Empty;
                            orderLine.UnitPrice = sparePart != null ? sparePart.FinalPrice.Value : 0;
                            orderLine.StrictlyThisNumber = false;
                            orderLine.CurrentStatus = 0;
                            orderLine.Processed = 0;
                            orderLine.OrderLineStatuses = orderStatus;

                            respOrder.OrderName = order.OrderName;
                            respOrder.PartNumbers.Add(respLine);
                            dbOrder.OrderLines.Add(orderLine);
                        }
                        else
                        {
                            respLine.Status = ResponsePartNumber.NotFound;
                            respOrder.PartNumbers.Add(respLine);
                        }
                    }
                    respOrder.Total = dbOrder.Total = total;

                    try
                    {
                        var createorder = dc.Orders.Add(dbOrder);
                        dc.SaveChanges();
                        dbTransaction.Commit();
                        if (dbOrder.OrderID != 0)
                        {
                            respOrder.OrderDate = dbOrder.OrderDate;
                            respOrder.OrderId = dbOrder.OrderID;
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

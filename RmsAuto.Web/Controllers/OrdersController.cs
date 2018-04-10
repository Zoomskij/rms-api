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

        public Users CurrentUser { get; set; }

        [HttpGet]
        [Route("orders")]
        public IHttpActionResult GetOrders()
        {
            CurrentUser = db.Users.FirstOrDefault(x => x.Username == "api" || x.Email == "api");
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
        public IHttpActionResult GetOrder(int orderId)
        {
            var order = db.Orders.FirstOrDefault(x => x.OrderID == orderId);
            if (order == null) return NotFound();
            var userOrder = Mapper.Map<Orders, Order<PartNumber>>(order);

            var orderLines = Mapper.Map<ICollection<OrderLines>, List<PartNumber>>(order.OrderLines);

            return Ok(userOrder);
        }

        [HttpPost]
        [Route("orders")]
        public IHttpActionResult CreateOrder([FromBody] Order<OrderPartNumbers> order)
        {
            using (var dc = new ex_rmsauto_storeEntities())
            {
                CurrentUser = dc.Users.FirstOrDefault(x => x.Username == "api" || x.Email == "api");
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

                    foreach (var pn in order.PartNumbers)
                    {
                        var ol = Mapper.Map<OrderPartNumbers, OrderLines>(pn);
                        var sparePart = dc.spGetSparePart(pn.Brand, pn.Article, pn.SupplierID, CurrentUser.AcctgID).FirstOrDefault();
                        if (sparePart != null)
                        {
                            switch (pn.ReactionByCount)
                            {
                                case 0:
                                    if (sparePart?.QtyInStock < pn?.Count)
                                    {
                                        continue;
                                    }
                                    break;
                                case 1:
                                    if (sparePart?.QtyInStock < pn?.Count)
                                    {
                                        ol.Qty = sparePart.QtyInStock.Value;
                                    }
                                    break;
                                case 2:
                                    if (sparePart?.QtyInStock < pn?.Count && sparePart.MinOrderQty.HasValue)
                                    {
                                        ol.Qty = pn.Count.Value + sparePart.MinOrderQty.Value - (pn.Count.Value % sparePart.MinOrderQty.Value);
                                    }
                                    break;
                            }
                            
                        }
                        else
                        {
                            // wrong
                        }



                    }

                    ////////
                    List<spGetSparePart_Result> spareParts = new List<spGetSparePart_Result>();

                    var orderLines = Mapper.Map<List<OrderPartNumbers>, ICollection<OrderLines>>(order.PartNumbers);
                    foreach (var ol in orderLines)
                    {
                        var sparePart = dc.spGetSparePart(ol.Manufacturer, ol.PartNumber, ol.SupplierID, CurrentUser.AcctgID).ToList().FirstOrDefault();
                        if (sparePart != null)
                        {
                            spareParts.Add(sparePart);
                            if (sparePart.FinalPrice != null)
                                total += sparePart.FinalPrice.Value * ol.Qty;
                        }

                        ol.DeliveryDaysMin = sparePart != null ? sparePart.DeliveryDaysMin : 0;
                        ol.DeliveryDaysMax = sparePart != null ? sparePart.DeliveryDaysMax ?? 0 : 0;
                        ol.PartName = sparePart != null ? sparePart.PartName : string.Empty;
                        ol.UnitPrice = sparePart != null ? sparePart.FinalPrice.Value : 0;
                        ol.StrictlyThisNumber = false;
                        ol.CurrentStatus = 0;
                        ol.Processed = 0;
                        ol.OrderLineStatuses = orderStatus;

                        var spareResp = Mapper.Map<OrderLines, ResponsePartNumbers>(ol);
                    }

                    dbOrder.Total = total;

                    dbOrder.OrderLines = orderLines;

                    // dc.Database.tra DataContext.Transaction = dc.DataContext.Connection.BeginTransaction();
                    try
                    {
                        var createorder = dc.Orders.Add(dbOrder);
                        //dc.SaveChanges();
                       // dbTransaction.Commit();
                        if (dbOrder.OrderID == 0)
                        {
                            var createdOrder = Mapper.Map<Orders, Order<ResponsePartNumbers>>(dbOrder);
                            var parts = Mapper.Map<ICollection<OrderLines>, List<ResponsePartNumbers>>(dbOrder.OrderLines);

                            foreach (var part in parts)
                            {
                                if (string.IsNullOrWhiteSpace(part.Article) || string.IsNullOrWhiteSpace(part.Brand) || part.SupplierID == 0)
                                {
                                    part.Status = ResponsePartNumber.NotApproved;
                                }
                                if (part.CountOrder == 0)
                                {
                                    part.Status = ResponsePartNumber.NotCount;
                                }




                                var sparePart = spareParts.FirstOrDefault(x => x.Manufacturer.Equals(part.Brand) && x.PartNumber.Equals(part.Article) && x.SupplierID == part.SupplierID);
                                if (sparePart != null)
                                {
                                    int? allowedCount = sparePart.QtyInStock;
                                    if (allowedCount != null)
                                    {
                                        if (part.CountOrder > allowedCount)
                                        {
                                            part.Status = ResponsePartNumber.NotSetReactionByCount;
                                        }
                                    }
                                }
                                else
                                {
                                    part.Status = ResponsePartNumber.NotFound;
                                }
                            }

                            createdOrder.PartNumbers = parts;
                            return Ok(createdOrder);
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

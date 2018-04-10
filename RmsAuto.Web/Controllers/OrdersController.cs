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
            //var order = JsonConvert.DeserializeObject<Order<OrderPartNumbers>>(orderz.ToString());
            using (var dc = new ex_rmsauto_storeEntities())
            {
                using (var dbTransaction = dc.Database.BeginTransaction())
                {
                    CurrentUser = dc.Users.FirstOrDefault(x => x.Username == "api" || x.Email == "api");


                    var orderStatus = dc.OrderLineStatuses.FirstOrDefault(x => x.OrderLineStatusID == 10);

                    decimal total = 0;
                    List<spGetSparePart_Result> spareParts = new List<spGetSparePart_Result>();

                    Orders dbOrder = new Orders();
                    dbOrder.UserID = CurrentUser.UserID;
                    dbOrder.ClientID = CurrentUser.AcctgID;
                    dbOrder.StoreNumber = "StoreNumber";
                    dbOrder.ShippingMethod = 0;
                    dbOrder.PaymentMethod = (byte)PaymentMethod.Cash;
                    dbOrder.Status = 0;
                    dbOrder.OrderDate = DateTime.Now;
                   
                    dbOrder.Users = CurrentUser;

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

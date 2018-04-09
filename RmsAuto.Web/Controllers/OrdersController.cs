using AutoMapper;
using RMSAutoAPI.App_Data;
using RMSAutoAPI.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace RMSAutoAPI.Controllers
{
    [RoutePrefix("api")]
    public class OrdersController : ApiController
    {
        private ex_rmsauto_storeEntities db = new ex_rmsauto_storeEntities();

        public Users CurrentUser { get; set; }

        [HttpGet]
        [Route("orders/{orderId}")]
        public IHttpActionResult GetOrders(int orderId)
        {
            var order = db.Orders.FirstOrDefault(x => x.OrderID == orderId);
            if (order == null) return NotFound();
            Order<PartNumber> newOrder = new Order<PartNumber>();

            newOrder.OrderId = order.OrderID;
            newOrder.Username = order.Users.Username;
            newOrder.OrderDate = order.OrderDate;
            newOrder.CompletedDate = order.CompletedDate;
            newOrder.Status = order.Status;
            newOrder.Total = order.Total;

            foreach (var ol in order.OrderLines)
            {
                PartNumber pn = new PartNumber();
                pn.Article = ol.PartNumber;
                pn.Brand = ol.Manufacturer;
                pn.SupplierID = ol.SupplierID;
                pn.Count = ol.Qty;
                pn.Price = ol.UnitPrice;
                pn.Name = ol.PartName;
                newOrder.PartNumbers.Add(pn);
            }

            return Ok(newOrder);
        }

        [HttpPost]
        [Route("orders")]

        public IHttpActionResult CreateOrderz([FromBody] Order<OrderPartNumbers> order)
        {
            using (var dc = new ex_rmsauto_storeEntities())
            {
                using (var dbTransaction = dc.Database.BeginTransaction())
                {
                    CurrentUser = dc.Users.FirstOrDefault(x => x.Username == "api" || x.Email == "api");
                    var articles = @"'<b M=""AC DELCO"" P=""41803"" S=""8735"" />'";


                    var prices = dc.spGetCartSpareParts(articles, CurrentUser.AcctgID, null, null);

                    var orderStatus = dc.OrderLineStatuses.FirstOrDefault(x => x.OrderLineStatusID == 10);

                    decimal total = 0;
                    foreach (var pn in order.PartNumbers)
                    {
                        var summary = dc.SpareParts.FirstOrDefault(x =>
                             x.SupplierID == pn.SupplierID &&
                             x.Manufacturer == pn.Brand &&
                             x.PartNumber == pn.Article);
                        if (summary != null && pn.Count.HasValue)
                        {
                            total += pn.Count.Value * summary.InitialPrice;
                        }
                    }

                    Orders dbOrder = new Orders();
                    dbOrder.UserID = CurrentUser.UserID;
                    dbOrder.ClientID = CurrentUser.AcctgID;
                    dbOrder.StoreNumber = "StoreNumber";
                    dbOrder.ShippingMethod = 0;
                    dbOrder.PaymentMethod = (byte)PaymentMethod.Cash;
                    dbOrder.Status = 0;
                    dbOrder.OrderDate = DateTime.Now;
                    dbOrder.Total = total;
                    dbOrder.Users = CurrentUser;

                    var orderLines = Mapper.Map<List<OrderPartNumbers>, ICollection<OrderLines>>(order.PartNumbers);
                    foreach (var ol in orderLines)
                    {
                        ol.DeliveryDaysMin = 0;
                        ol.DeliveryDaysMax = 0;
                        ol.PartName = "test";
                        ol.UnitPrice = Convert.ToDecimal(0.00);
                        ol.StrictlyThisNumber = false;
                        ol.CurrentStatus = 0;
                        ol.Processed = 0;
                        ol.OrderLineStatuses = orderStatus;
                    }
                    dbOrder.OrderLines = orderLines;

                    // dc.Database.tra DataContext.Transaction = dc.DataContext.Connection.BeginTransaction();
                    try
                    {
                        var createorder = dc.Orders.Add(dbOrder);
                        dc.SaveChanges();
                        dbTransaction.Commit();
                        if (dbOrder.OrderID != 0)
                        {
                            var createdOrder = Mapper.Map<Orders, Order<OrderResponsePartNumbers>>(dbOrder);
                            var parts = Mapper.Map<ICollection<OrderLines>, List<OrderResponsePartNumbers>>(dbOrder.OrderLines);

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
    }
}

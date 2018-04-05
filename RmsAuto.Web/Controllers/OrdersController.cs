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
            Order newOrder = new Order();

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

        public IHttpActionResult CreateOrderz([FromBody] List<PartNumber> partNumbers)
        {
            CurrentUser = db.Users.FirstOrDefault(x => x.Username == "api" || x.Email == "api");

            var orderStatus = db.OrderLineStatuses.FirstOrDefault(x => x.OrderLineStatusID == 10);

            decimal total = 0;
            foreach (var pn in partNumbers)
            {
                var summary = db.SpareParts.FirstOrDefault(x =>
                     x.SupplierID == pn.SupplierID &&
                     x.Manufacturer == pn.Brand &&
                     x.PartNumber == pn.Article);
                if (summary != null && pn.Count.HasValue)
                {
                    total += pn.Count.Value * summary.InitialPrice;
                }
            }


            Orders order = new Orders();
            order.UserID = CurrentUser.UserID;
            order.ClientID = CurrentUser.AcctgID;
            order.ShippingMethod = 0;
            order.PaymentMethod = 0;
            order.StoreNumber = "StoreNumber";
            order.Status = 0;
            order.OrderDate = DateTime.Now;
            order.Total = total;
            order.Users = CurrentUser;

            var orderLines = Mapper.Map< List<PartNumber>,ICollection <OrderLines>>(partNumbers);
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
            order.OrderLines = orderLines;

            try
            {
                var createorder = db.Orders.Add(order);
                db.SaveChanges();
                if (order.OrderID != 0)
                {
                    var createdOrder = Mapper.Map<Orders, Order>(order);
                    var parts = Mapper.Map<ICollection<OrderLines>, List<PartNumber>>(order.OrderLines);
                    createdOrder.PartNumbers = parts;
                    return Ok(createdOrder);
                }

            }
            catch (DbEntityValidationException e)
            {
              
            }

            return Ok(order);
        }
    }
}

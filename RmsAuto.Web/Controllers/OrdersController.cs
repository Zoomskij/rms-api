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

        public IHttpActionResult CreateOrderz([FromBody] PartNumber partNumber)
        {
            CurrentUser = db.Users.FirstOrDefault(x => x.Username == "api" || x.Email == "api");


            var summary = db.SpareParts.FirstOrDefault(x => 
                     x.SupplierID == partNumber.SupplierID &&
                     x.Manufacturer == partNumber.Brand &&
                     x.PartNumber == partNumber.Article);
            if (summary == null) return NotFound();
            
            if (summary.QtyInStock < partNumber.Count) return Content(HttpStatusCode.BadRequest, "Too many");

            var orderStatus = db.OrderLineStatuses.FirstOrDefault(x => x.OrderLineStatusID == 10);

            Orders order = new Orders();
            order.UserID = CurrentUser.UserID;
            order.ClientID = CurrentUser.AcctgID;
            order.ShippingMethod = 0;
            order.PaymentMethod = 0;
            order.StoreNumber = "StoreNumber";
            order.Status = 0;
            order.OrderDate = DateTime.Now;
            order.Total = summary.InitialPrice * partNumber.Count.Value;
            order.Users = CurrentUser;
            order.OrderLines.Add(new OrderLines
            {
                //OrderID = order.OrderID,
                PartNumber = partNumber.Article,
                Manufacturer = partNumber.Brand,
                SupplierID = partNumber.SupplierID,
                Qty = partNumber.Count.Value,
                DeliveryDaysMax = 0,
                DeliveryDaysMin = 0,
                PartName = "test",
                UnitPrice = Convert.ToDecimal(0.00),
                StrictlyThisNumber = false,
                CurrentStatus = 0,
                Processed = 0,
                OrderLineStatuses = orderStatus
            });
            db.Orders.Add(order);
            try
            {
                db.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }


            return Ok(partNumber);
        }
    }
}

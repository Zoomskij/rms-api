using RMSAutoAPI.App_Data;
using RMSAutoAPI.Models;
using System;
using System.Collections.Generic;
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

        [HttpGet]
        [Route("orders/{orderId}")]
        public IHttpActionResult GetOrders(int orderId)
        {
            var order = db.Orders.FirstOrDefault(x => x.OrderID == orderId);
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
    }
}

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
        //[Authorize(Roles = "Client_SearchApi, NoAccess")]
        public IHttpActionResult CreateOrder([FromBody] Order<OrderSparePart> order)
        {
            Order<ResponseSparePart> RespOrder = new Order<ResponseSparePart>();
            DbOrder = new Orders();
            using (var dbTransaction = db.Database.BeginTransaction())
            {
                var orderLineStatus = db.OrderLineStatuses.FirstOrDefault(x => x.OrderLineStatusID == 10);

                DbOrder = Mapper.Map<Order<OrderSparePart>, Orders>(order);
                
                foreach (var ol in DbOrder.OrderLines)
                {
                    var sparePart = db.spGetSparePart(ol.Manufacturer, ol.PartNumber, ol.SupplierID, CurrentUser.AcctgID).FirstOrDefault();
                    var pn = order.SpareParts.FirstOrDefault(x => x.Article == ol.PartNumber && x.Brand == ol.Manufacturer && x.SupplierID == ol.SupplierID && x.Reference == ol.ReferenceID);
                    switch (order.Reaction)
                    {
                        case Reaction.AnyPush:
                            ol.Qty = GetQtyValue(ol.Qty, sparePart.QtyInStock, sparePart.MinOrderQty);
                            ol.UnitPrice = sparePart.FinalPrice ?? sparePart.InitialPrice;
                            break;
                        case Reaction.NotPush: 
                            if (sparePart?.QtyInStock < ol.Qty)
                                return BadRequest();
                            if (ol.UnitPrice < sparePart?.FinalPrice)
                                return BadRequest();
                            break;
                        case Reaction.CheckRow:
                            
                            switch (pn.ReactionByCount)
                            {
                                case 0:
                                    if (ol.Qty > sparePart.QtyInStock)
                                        ol.Qty = -1;
                                    break;
                                case 1:
                                    if (ol.Qty < sparePart.QtyInStock)
                                    {
                                        ol.Qty = GetQtyValue(ol.Qty, sparePart.QtyInStock, sparePart.MinOrderQty);
                                    }
                                    break;
                                case 2:
                                    ol.Qty = GetQtyValue(ol.Qty, sparePart.QtyInStock, sparePart.MinOrderQty);
                                    break;
                            }
                            switch (pn.ReactionByPrice)
                            {
                                case 0:
                                    if (sparePart?.FinalPrice > ol.UnitPrice)
                                    {
                                        ol.UnitPrice = -1;
                                    }
                                    ol.UnitPrice = Math.Round(sparePart.FinalPrice.Value, 2);
                                    break;
                                case 1:
                                    if (sparePart.FinalPrice.HasValue)
                                    {
                                        ol.UnitPrice = Math.Round(sparePart.FinalPrice.Value, 2);
                                    }
                                    break;
                            }
                            break;
                    }
                    ol.DeliveryDaysMin = sparePart != null ? sparePart.DeliveryDaysMin : 0;
                    ol.DeliveryDaysMax = sparePart != null ? sparePart.DeliveryDaysMax ?? 0 : 0;
                    ol.PartName = sparePart != null ? sparePart.PartName : string.Empty;
                    ol.UnitPrice = sparePart != null ? Math.Round(sparePart.FinalPrice.Value, 2) : 0;
                    ol.StrictlyThisNumber = false;
                    ol.CurrentStatus = 0;
                    ol.Processed = 0;
                    ol.OrderLineStatuses = orderLineStatus;
                    DbOrder.Total += ol.Qty == -1 ? 0 : sparePart.FinalPrice * ol.Qty ?? 0;
                }     

                try
                {
                    RespOrder = Mapper.Map<Orders, Order<ResponseSparePart>>(DbOrder);
                    foreach (var sp in RespOrder.SpareParts)
                    {
                        var sparePart = order.SpareParts.FirstOrDefault(x => x.Brand == sp.Brand && x.Article == sp.Article && x.SupplierID == sp.SupplierID && x.Reference == sp.Reference);
                        sp.PriceOrder = sparePart.Price;
                        sp.CountOrder = sparePart.Count ?? 0;
                        if (sp.PriceApproved == -1)
                        {
                            sp.Status = ResponsePartNumber.WrongPrice;
                            sp.PriceApproved = 0;
                        }
                        if (sp.CountApproved == -1)
                        {
                            sp.Status = ResponsePartNumber.SomethingCount;
                            sp.CountApproved = 0;
                        }

                    }

                    var dbLines = DbOrder.OrderLines.Where(x => x.UnitPrice == -1 || x.Qty == -1).ToList();
                    foreach (var orderLine in dbLines.ToList())
                        DbOrder.OrderLines.Remove(orderLine);
                    var createorder = db.Orders.Add(DbOrder);

                    if (DbOrder.OrderLines.Any())
                    {
                        db.SaveChanges();
                        if (DbOrder.OrderID != 0)
                        {
                            var orderHelper = new OrderHelper(db);
                            RespOrder.OrderId = DbOrder.OrderID;
                            orderHelper.SendOrder(DbOrder, string.Empty);
                            dbTransaction.Commit();
                            return Ok(RespOrder);
                        }
                    }
                }
                catch (DbEntityValidationException e)
                {
                    dbTransaction.Rollback();
                }
            }

            return Ok(RespOrder);
        }

        public int GetQtyValue(int orderCount, int? stockCount, int? min)
        {
            if (min.HasValue && min != 1)
            {
                if (stockCount.Value + min < orderCount)
                {
                    return stockCount.Value - min.Value + (stockCount.Value % min.Value);
                }
                return orderCount + min.Value - (orderCount % min.Value);
            }
            if (orderCount < stockCount.Value)
                return orderCount;
            else return stockCount.Value;
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

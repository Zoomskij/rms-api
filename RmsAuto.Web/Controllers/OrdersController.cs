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
using System.Threading.Tasks;
using System.Web.Http;

namespace RMSAutoAPI.Controllers
{
    [RoutePrefix("api")]
    public class OrdersController : ApiController
    {
        private ex_rmsauto_storeEntities db = new ex_rmsauto_storeEntities();
        public static Users CurrentUser { get; set; }
        private Orders _dbOrder;
        private string _region;
        public Orders DbOrder
        {
            get => _dbOrder;
            set
            {
                _dbOrder = value;
                _dbOrder.UserID = CurrentUser.UserID;
                _dbOrder.ClientID = CurrentUser.AcctgID;
                _dbOrder.Users = CurrentUser;
                _dbOrder.OrderDate = DateTime.Now;
                _dbOrder.Status = 0;
                _dbOrder.PaymentMethod = (byte)PaymentMethod.Cash;
                _dbOrder.ShippingMethod = 1;
                _dbOrder.StoreNumber = "1";
            }
        }

        public OrdersController()
        {
            var claims = (ClaimsIdentity)User.Identity;
            _region = claims.Claims.FirstOrDefault(x => x.Type.Equals("Region"))?.Value;

            if (!string.IsNullOrWhiteSpace(_region))
            {
                var currentFranch = db.spGetFranches().FirstOrDefault(x => x.InternalFranchName.ToUpper().Equals(_region.ToUpper()));
                db.ChangeDatabase(initialCatalog: $"ex_{currentFranch.DbName}_store", dataSource: $"{currentFranch.ServerName}");
            }
            else db = new ex_rmsauto_storeEntities();

            var userName = User.Identity.Name;
            CurrentUser = db.Users.FirstOrDefault(x => x.Username == userName);
        }

        //[HttpGet]
        //[Route("orders")]
        //[Authorize]
        ////[Authorize(Roles = "Client_SearchApi, NoAccess")]
        //public IHttpActionResult GetOrders()
        //{
        //    var orders = db.Orders.Where(x => x.UserID == CurrentUser.UserID);
        //    if (!orders.Any()) return Ok(new List<Orders>());
        //    var userOrders = new List<Order<SparePart>>();

        //    foreach (var order in orders)
        //    {
        //        var newOrder = Mapper.Map<Orders, Order<SparePart>>(order);

        //        userOrders.Add(newOrder);
        //    }
        //    return Ok(userOrders);
        //}

        [HttpGet]
        [Route("orders/{orderId}")]
        [Authorize(Roles = "Create_Order")]
        public IHttpActionResult GetOrder(int orderId)
        {
            var userName = User.Identity.Name;

            var order = db.Orders.FirstOrDefault(x => x.OrderID == orderId && x.UserID == CurrentUser.UserID);
            if (order == null) return Content(HttpStatusCode.NotFound, Resources.ErrorNotFound); 
            var userOrder = Mapper.Map<Orders, Order>(order);

            var orderLines = Mapper.Map<ICollection<OrderLines>, List<OrderLine>>(order.OrderLines);

            return Ok(userOrder);
        }

        [HttpPost]
        [Route("orders")]
        [Authorize(Roles = "Create_Order")]
        public IHttpActionResult CreateOrder([FromBody] OrderHead orderHead)
        {
            if (orderHead == null)
            {
                return Content(HttpStatusCode.BadRequest, "Bad Request");
            }
            if ((int)orderHead.ValidationType < 0 || (int)orderHead.ValidationType >= 3)
            {
                return Content(HttpStatusCode.BadRequest, "ValidationType should be is [0, 1, 2]");
            }
            int countUp = 0;
            int countLess = 0;
            DbOrder = new Orders();
            DbOrder.IsTest = orderHead.IsTest;
            var respOrder = new OrderPlaced();
            using (var dbTransaction = db.Database.BeginTransaction())
            {
                var orderLineStatus = db.OrderLineStatuses.FirstOrDefault(x => x.OrderLineStatusID == 10);

                var orderXml = string.Empty;
                foreach (var sparePart in orderHead.OrderHeadLines)
                {
                    if (orderHead.ValidationType == Reaction.AnyPush)
                        orderXml += $"<b S=\"{sparePart.SupplierID}\" M=\"{sparePart.Brand}\" P=\"{sparePart.Article}\" R=\"{sparePart.Reference}\" />";
                    else
                        orderXml += $"<b S=\"{sparePart.SupplierID}\" M=\"{sparePart.Brand}\" P=\"{sparePart.Article}\" C=\"{sparePart.Price.ToString("0.00")}\" R=\"{sparePart.Reference}\" />";

                }
                orderXml = orderXml.Replace(",", "."); // Fast fix for replacing dots

                var calcLines = new List<CalcOrderLines>();
                {
                    if (string.IsNullOrWhiteSpace(_region))
                    {
                        var getcalcLines = db.Database.SqlQuery<CalcOrderLines>($"exec api.spCalcOrder '{orderXml}', '{CurrentUser.AcctgID}', NULL, NULL");
                        if (getcalcLines.Any())
                            calcLines = getcalcLines.ToList();
                    }
                    else
                    {
                        var getcalcLines = db.Database.SqlQuery<CalcOrderLines>($"exec api.spCalcOrder '{orderXml}', '{CurrentUser.AcctgID}', {CurrentUser.ClientGroup}, {_region}");
                        if (getcalcLines.Any())
                            calcLines = getcalcLines.ToList();
                    }
                }


                foreach (var sparePart in orderHead.OrderHeadLines)
                {
                    var dbOrderLine = new OrderLines();
                    var respOrderLine = new OrderPlacedLine
                    {
                        PriceOrder = sparePart.Price,
                        CountOrder = sparePart.Count
                    };

                    dbOrderLine.PartNumber = respOrderLine.Article = sparePart.Article;
                    dbOrderLine.Manufacturer = respOrderLine.Brand = sparePart.Brand;
                    dbOrderLine.SupplierID = respOrderLine.SupplierID = sparePart.SupplierID;
                    dbOrderLine.ReferenceID = respOrderLine.Reference = sparePart.Reference;

                    //Обрабатываем ошибки
                    if (string.IsNullOrWhiteSpace(sparePart.Article)  || string.IsNullOrWhiteSpace(sparePart.Brand) || sparePart.SupplierID == 0)
                    {
                        respOrderLine.Status = ResponsePartNumber.ErrorKey;
                        respOrder.OrderPlacedLines.Add(respOrderLine);
                        continue;
                    }
                    if (sparePart.Count <= 0)
                    {
                        respOrderLine.Status = ResponsePartNumber.CountNotSet;
                        respOrder.OrderPlacedLines.Add(respOrderLine);
                        continue;
                    }
                    //Проверяем коллизию по количеству
                    if (orderHead.ValidationType == Reaction.CheckRow && ((sparePart.ReactionByCount < 0) || (sparePart.ReactionByCount > 3)))
                    {
                        respOrderLine.Status = ResponsePartNumber.NotSetReactionByCount;
                        respOrder.OrderPlacedLines.Add(respOrderLine);
                        continue;
                    }
                    if (orderHead.ValidationType == Reaction.CheckRow && ((sparePart.ReactionByPrice < 0) || (sparePart.ReactionByPrice > 1)))
                    {
                        respOrderLine.Status = ResponsePartNumber.NotSetReactionByPrice;
                        respOrder.OrderPlacedLines.Add(respOrderLine);
                        continue;
                    }

                    var part = calcLines.FirstOrDefault(x => x.Manufacturer == sparePart.Brand && x.PartNumber == sparePart.Article && x.SupplierID == sparePart.SupplierID);
                    if (part == null)
                    {
                        respOrderLine.Status = ResponsePartNumber.NotFound;
                        respOrder.OrderPlacedLines.Add(respOrderLine);
                        continue;
                    }
                    if (part.QtyInStock == null)
                    {
                        respOrderLine.Status = ResponsePartNumber.NotFound;
                        respOrder.OrderPlacedLines.Add(respOrderLine);
                        continue;
                    }
                    if (part.FinalPrice == null)
                    {
                        respOrderLine.Status = ResponsePartNumber.WrongPrice;
                        respOrder.OrderPlacedLines.Add(respOrderLine);
                        continue;
                    }

                    switch (orderHead.ValidationType)
                    {
                        case Reaction.NotErrorPush:
                            if (sparePart.Count > part.QtyInStock)
                            {
                                respOrderLine.Status = ResponsePartNumber.ErrorCount;
                                break;
                            }
                            respOrderLine.PricePlaced = Math.Round(part.FinalPrice.Value, 2);
                            respOrderLine.CountPlaced = sparePart.Count;
                            break;
                        case Reaction.AnyPush:
                            respOrderLine.PriceOrder = sparePart.Price;
                            countUp = GetMoreMinQty(sparePart.Count, part.MinOrderQty, part.QtyInStock.Value);
                            countLess = GetLessMinQty(sparePart.Count, part.MinOrderQty, part.QtyInStock.Value);
                            respOrderLine.CountPlaced = countUp > part.QtyInStock ? countLess : countUp;
                            respOrderLine.PricePlaced = dbOrderLine.UnitPrice = Math.Round(part.FinalPrice.Value, 2);
                            //Если произошло снижение по остаткам поставщика
                            if (respOrderLine.CountPlaced < sparePart.Count)
                                respOrderLine.Status = ResponsePartNumber.OkCountLess;
                            //Если произошло выравнивание вверх по MinQty
                            if (respOrderLine.CountPlaced > sparePart.Count)
                                respOrderLine.Status = ResponsePartNumber.OkCountMoreQty;
                            break;
                        case Reaction.CheckRow:
                            switch (sparePart.ReactionByCount)
                            {
                                // берем только указанное количество
                                case 0:
                                    if (sparePart.Count > part.QtyInStock)
                                    {
                                        respOrderLine.Status = ResponsePartNumber.ErrorCount;
                                        respOrder.OrderPlacedLines.Add(respOrderLine);
                                        continue;
                                    }
                                    else
                                    {
                                       respOrderLine.CountPlaced = respOrderLine.CountOrder = sparePart.Count;
                                    }
                                    break;
                                // Берем сколько есть, но не выше указанного
                                case 1:
                                    if (sparePart.Count > part.QtyInStock)
                                    {
                                        respOrderLine.CountPlaced = part.QtyInStock.Value;
                                        respOrderLine.Status = ResponsePartNumber.OkCountLess;
                                    }
                                    else
                                    {
                                       respOrderLine.CountPlaced = sparePart.Count;
                                    }
                                    break;
                                // Разрешаем выравнивать вверх по MinQty
                                case 2:
                                    countUp = GetMoreMinQty(sparePart.Count, part.MinOrderQty, part.QtyInStock.Value);
                                    if (countUp > part.QtyInStock)
                                    {
                                        respOrderLine.Status = ResponsePartNumber.ErrorCount;
                                        respOrder.OrderPlacedLines.Add(respOrderLine);
                                        continue;
                                    }
                                    respOrderLine.CountPlaced = countUp;
                                    respOrderLine.Status = ResponsePartNumber.OkCountMoreQty;
                                    break;
                                // Разрешаем выравнивать вниз по MinQty
                                case 3:
                                    respOrderLine.CountPlaced = GetLessMinQty(sparePart.Count, part.MinOrderQty, part.QtyInStock.Value);
                                    respOrderLine.Status = ResponsePartNumber.OkCountLessQty;
                                    break;
                            }

                            switch (sparePart.ReactionByPrice)
                            {
                                // Не выше указанной цены
                                case 0:
                                    respOrderLine.PricePlaced = Math.Round(part.FinalPrice.Value, 2);
                                    break;
                                // текущая цена поставщика (без проверки)
                                case 1:
                                    respOrderLine.PricePlaced = Math.Round(part.FinalPrice.Value, 2);
                                    break;
                            }
                            break;
                    }

                    if ((int)orderHead.ValidationType >= 0 && (int)orderHead.ValidationType <= 2)
                    {
                        dbOrderLine.Qty = respOrderLine.CountPlaced;
                        dbOrderLine.DeliveryDaysMin = sparePart != null ? part.DeliveryDaysMin.Value : 0;
                        dbOrderLine.DeliveryDaysMax = sparePart != null ? part.DeliveryDaysMax.Value : 0;
                        dbOrderLine.PartName = sparePart != null ? part.PartName : string.Empty;
                        dbOrderLine.UnitPrice = respOrderLine.PricePlaced;
                        dbOrderLine.StrictlyThisNumber = sparePart.StrictlyThisNumber;
                        dbOrderLine.CurrentStatus = 0;
                        dbOrderLine.Processed = 0;
                        dbOrderLine.OrderLineStatuses = orderLineStatus;
                        dbOrderLine.AcctgOrderLineID = dbOrderLine.OrderLineID * -1;
                        DbOrder.Total += Math.Round(part.FinalPrice.Value, 2) * dbOrderLine.Qty;

                        DbOrder.OrderLines.Add(dbOrderLine);


                        respOrder.OrderPlacedLines.Add(respOrderLine);
                    }
                }

                DbOrder.CustOrderNum = orderHead.CustOrderNum;
                DbOrder.OrderNotes = orderHead.OrderNotes;

                if (orderHead.ValidationType == 0)
                {
                    if (!respOrder.OrderPlacedLines.Any() || respOrder.OrderPlacedLines.Any(x => x.Status != 0))
                        respOrder.Status = OrderStatus.Error; // заказ не размещен
                }

                if (orderHead.ValidationType != 0 && !DbOrder.OrderLines.Any())
                {
                    respOrder.Status = OrderStatus.Error;  // заказ не размещен
                }
                if (orderHead.ValidationType != 0 && DbOrder.OrderLines.Any() && respOrder.OrderPlacedLines.All(x => x.Status == 0))
                {
                    respOrder.Status = OrderStatus.OkPart; // заказ размещен частично
                }

                try
                {
                    if (respOrder.Status != OrderStatus.Error)
                    { 
                        respOrder.Total = DbOrder.Total;
                        var createorder = db.Orders.Add(DbOrder);

                        if (respOrder.OrderPlacedLines.Any(x => x.Status != ResponsePartNumber.Ok))
                        {
                            respOrder.Status = OrderStatus.OkPart;
                        }
                        db.SaveChanges();
                        if (DbOrder.OrderID != 0)
                        {
                            if (orderHead.IsTest == false)
                            {
                                var orderHelper = new OrderHelper(db);
                                orderHelper.SendOrder(DbOrder, string.Empty);
                            }
                            dbTransaction.Commit();

                            respOrder.OrderId = DbOrder.OrderID;
                            respOrder.Status = 0;

                            //Делаем логирование
                            Task.Run(() =>
                            {
                                var logOrder = Mapper.Map<OrderPlaced, OrderHistory>(respOrder);
                                logOrder.UserId = CurrentUser.UserID;
                                foreach (var line in logOrder.OrderHistoryDetail)
                                {
                                    line.OrderId = logOrder.OrderId;
                                    line.OrderHistory = logOrder;
                                }
                                db.OrderHistory.Add(logOrder);
                                db.SaveChanges();
                            });       

                            return Ok(respOrder);
                        }

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
            if (!minQty.HasValue) return orderCount;
            var value = orderCount + (orderCount % minQty.Value);
            return value;
        }

        public int GetLessMinQty(int orderCount, int? minQty, int stockCount)
        {
            if (!minQty.HasValue) return orderCount;
            var value = orderCount - (orderCount % minQty.Value);
            if (value < stockCount)
                return value;
            else
            {
                return stockCount - (stockCount % minQty.Value);
            }
        }

        public static string ConvertManufacturerToSP(string manufacturer)
        {
            var reg = new Regex("[&><\"]");
            if (!reg.IsMatch(manufacturer)) return manufacturer;
            var replaces = new Dictionary<string, string> {{"&", "&amp;"}, {"\"", "&quot;"}, {">", "&gt;"}, {"<", "&lt;"}};

            foreach (var ch in replaces)
                manufacturer = manufacturer.Replace(ch.Key, ch.Value);
            return manufacturer;
        }
    }
}

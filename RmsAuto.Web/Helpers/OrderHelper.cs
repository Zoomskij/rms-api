using RMSAutoAPI.App_Data;
using RMSAutoAPI.Models;
using RMSAutoAPI.Models.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace RMSAutoAPI.Helpers
{
    public class OrderHelper
    {
        ex_rmsauto_storeEntities _db;
        public OrderHelper(ex_rmsauto_storeEntities db)
        {
            _db = db;
        }
        public void SendOrder(Orders order, string employeeId)
        {
            if (order == null) throw new ArgumentNullException("order");

            var perm1C = CommonDac.GetPermutations(); //Заполняем словарь перестановок SupplierID

            var acctgOrder = new OrderInfo
            {
                ClientId = order.ClientID,
                OrderNo = order.OrderID.ToString(),
                CustOrderNum = order.CustOrderNum,
                OrderDate = order.OrderDate,
                DeliveryAddress = string.Empty,// order.ShippingAddress,
                PaymentType = string.Empty, //order.PaymentMethod.ToTextOrName(),
                EmployeeId = employeeId,
                OrderNotes = string.Empty, // order.OrderNotes,
                OrderLines = order.OrderLines.Select<OrderLines, OrderLineInfo>(
                    l => new OrderLineInfo
                    {
                        WebOrderLineId = l.OrderLineID,
                        Article = new ArticleInfo
                        {

                            PartNumber = l.PartNumber, //PartNumber = ProcessingPACK(l.PartNumber, l.SupplierID), //обрабатываем PACK
                            Manufacturer = l.Manufacturer,
                            /* Реализована возможность продавать одну и ту же деталь (pn, brand, supplierID) по разным ценам (например если при разной "партионности" разная цена, т.е. 1 шт. - 10р. 10 шт. - 9р.):
							 * в этом случае данная деталь заливается с разными SupplierID (реальный и "виртуальный"). Т.к. УС ничего не знает о данных "виртуальных" SupplierID, то при отправке в УС
							 * должна производиться подмена "виртуальных" SupplierID реальными. */
                            SupplierId = perm1C.ContainsKey(l.SupplierID) ? (int)perm1C[l.SupplierID] : l.SupplierID,
                            ReferenceID = l.ReferenceID,
                            DeliveryDaysMin = l.DeliveryDaysMin,
                            DeliveryDaysMax = l.DeliveryDaysMax,
                            Description = l.PartDescription,
                            DescriptionOrig = l.PartName,
                            InternalPartNumber = l.PartNumber, // l.Part.InternalPartNumber,
                            SupplierPriceWithMarkup = Convert.ToDecimal(l.SupplierPriceWithMarkup),
                            SupplierMarkup = 0, // l.Part.PriceConstantTerm.GetValueOrDefault(),
                            WeightPhysical = l.WeightPhysical.GetValueOrDefault(), // l.WeightPhysical.GetValueOrDefault(),
                            WeightVolume = l.WeightVolume.GetValueOrDefault(),
                            DiscountGroup = string.Empty // l.Part.RgCode
                        },
                        FinalSalePrice = l.UnitPrice,
                        Quantity = l.Qty,
                        StrictlyThisNumber = l.StrictlyThisNumber ? 1 : 0,
                        VinCheckupData = l.VinCheckupData,
                        OrderLineNotes = l.OrderLineNotes
                    }).ToArray()
            };

            SendOrder(ref acctgOrder);

            foreach (var wl in order.OrderLines)
            {
                var al = acctgOrder.OrderLines.Single(l => l.WebOrderLineId == wl.OrderLineID);
                wl.AcctgOrderLineID = al.AcctgOrderLineId;
            }

            //if (order.OrderLines.FirstOrDefault(l => !l.AcctgOrderLineID.HasValue) != null)
            //    throw new BLException("Ошибка отправки заказа. Не принято одна или более позиций заказа");
        }

        public void SendOrder(ref OrderInfo order)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            ServiceProxy proxy = new ServiceProxy(_db);
            proxy.SendOrder(order);
        }

    }
}
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.Script.Serialization;

namespace RMSAutoAPI.Models
{
    public class OrderPlaced
    {
		[Description("ID заказа")]
		public int OrderId { get; set; }

        [Description("Статус заказа:" +
			"\n0 - заказ полностью размещён" +
			"\n1 - заказ размещён частично (см. Строки заказа)" +
			"\n2 - заказ не размещён (см. Строки заказа)")]
        public OrderStatus Status { get; set; }
        [Description("Сумма заказа")]
        public decimal Total { get; set; }
        [Description("Строки заказа")]
        public List<OrderPlacedLine> OrderPlacedLines { get; set; } = new List<OrderPlacedLine>();
    }
}
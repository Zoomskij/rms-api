using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.Script.Serialization;

namespace RMSAutoAPI.Models
{
    public class OrderPlaced
    {
        public int OrderId { get; set; }

        public OrderStatus Status { get; set; }
        [Description("Сумма заказа")]
        public decimal Total { get; set; }
        [Description("Детали заказа")]
        public List<OrderPlacedLine> OrderPlacedLines { get; set; } = new List<OrderPlacedLine>();
    }
}
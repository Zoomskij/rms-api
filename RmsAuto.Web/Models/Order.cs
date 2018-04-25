using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace RMSAutoAPI.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        [Description("Описание")]
        public string Username { get; set; } = string.Empty;
        [Description("Описание")]
        public DateTime OrderDate { get; set; }
        [Description("Описание")]
        public DateTime CompletedDate { get; set; }
        [Description("Описание")]
        public byte Status { get; set; }
        [Description("Описание")]
        public decimal Total { get; set; }
        [Description("Описание")]
        public List<OrderLine> OrderLines { get; set; } = new List<OrderLine>();
    }
}
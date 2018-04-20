using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RMSAutoAPI.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public string Username { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public DateTime CompletedDate { get; set; }
        public byte Status { get; set; }
        public decimal Total { get; set; }
        public List<OrderLine> OrderLines { get; set; } = new List<OrderLine>();
    }
}
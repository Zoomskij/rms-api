using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RMSAutoAPI.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public string OrderName { get; set; }
        public Reaction Reaction { get; set; }
        public string Username { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public byte Status { get; set; }
        public decimal Total { get; set; }
        public List<PartNumber> PartNumbers { get; set; } = new List<PartNumber>();
    }
}
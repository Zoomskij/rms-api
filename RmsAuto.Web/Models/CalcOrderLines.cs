using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RMSAutoAPI.Models
{
    public class CalcOrderLines
    {
        public string Manufacturer { get; set; }
        public string PartNumber { get; set; }
        public string PartName { get; set; }
        public int SupplierID { get; set; }
        public int DeliveryDaysMin { get; set; }
        public int DeliveryDaysMax { get; set; }
        public decimal? OrderPrice { get; set; }
        public decimal CurrentPrice { get; set; }
        public decimal? FinalPrice { get; set; }
        public int QtyInStock { get; set; }
        public int MinOrderQty { get; set; }
        public decimal MinPrice { get; set; }
    }
}
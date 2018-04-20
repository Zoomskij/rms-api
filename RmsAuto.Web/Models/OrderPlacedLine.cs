using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace RMSAutoAPI.Models
{
    public class OrderPlacedLine : Part
    {
        [Description("Количество")]
        public int CountOrder { get; set; }
        [Description("Количество разрешенное")]
        public int CountPlaced { get; set; }
        [Description("Цена детали")]
        public decimal PriceOrder { get; set; }
        [Description("Цена детали подтвержденная")]
        public decimal PricePlaced { get; set; }
        [Description("Статус размещения")]
        public ResponsePartNumber Status { get; set; }
        [Description("Описание")]
        public string Reference { get; set; }
    }
}
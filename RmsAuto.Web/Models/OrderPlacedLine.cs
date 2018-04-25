using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace RMSAutoAPI.Models
{
    public class OrderPlacedLine : Part
    {
        [Description("Кол-во запрошенное клиентом")]
        public int CountOrder { get; set; }
        [Description("Кол-во размещённое в заказе")]
        public int CountPlaced { get; set; }
        [Description("Цена запрошенная клиентом")]
        public decimal PriceOrder { get; set; }
        [Description("Цена размещённая в заказе")]
        public decimal PricePlaced { get; set; }
        [Description("Статус размещения")]
        public ResponsePartNumber Status { get; set; }
        [Description("Код позиции")]
        public string Reference { get; set; }
    }
}
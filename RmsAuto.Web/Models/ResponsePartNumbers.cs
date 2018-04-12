﻿using System.ComponentModel;

namespace RMSAutoAPI.Models
{
    public class ResponsePartNumbers : Part
    {
        [Description("Количество")]
        public int CountOrder { get; set; }
        [Description("Количество разрешенное")]
        public int CountApproved { get; set; }
        [Description("Цена детали")]
        public decimal PriceOrder { get; set; }
        [Description("Цена детали подтвержденная")]
        public decimal PriceApproved { get; set; }
        [Description("Статус размещения")]
        public ResponsePartNumber Status { get; set; }
    }
}
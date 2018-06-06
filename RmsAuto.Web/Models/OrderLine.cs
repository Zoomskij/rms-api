using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RMSAutoAPI.Models
{
    public class OrderLine : Part
    {
        [Description("Описание артикула")]
        public string Name { get; set; }
        [Description("Заказанное количество")]
        public int? Count { get; set; }
        [Description("Цена")]
        public decimal Price { get; set; }
        [Description("Код позиции")]
        public string Reference { get; set; } = string.Empty;
    }
}
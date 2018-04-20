using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace RMSAutoAPI.Models
{
    public class OrderLine : Part
    {
        /// <summary>
        /// Описание артикула
        /// </summary>
        [Description("Описание артикула")]
        public string Name { get; set; }
        /// <summary>
        /// Заказанное количество
        /// </summary>
        [Description("Заказанное количество")]
        public int? Count { get; set; }
        /// <summary>
        /// Цена
        /// </summary>
        [Description("Цена")]
        public decimal Price { get; set; }
        [Description("Описание")]
        public string Reference { get; set; } = string.Empty;
    }
}
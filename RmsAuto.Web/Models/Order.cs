﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace RMSAutoAPI.Models
{
    public class Order
    {
		[Description("ID заказа")]
		public int OrderId { get; set; }
        [Description("Клиентский номер заказа")]
        public string CustOrderNum { get; set; } = string.Empty;
        [Description("Дата размещения заказа")]
        public DateTime OrderDate { get; set; }
        [Description("Дата завершения заказа")]
        public DateTime CompletedDate { get; set; }
        [Description("Статус заказа \n1 - новый \n2 - в работе \n3 - завершён")]
        public byte Status { get; set; }
        [Description("Сумма заказа")]
        public decimal Total { get; set; }
        [Description("Строки заказа")]
        public List<OrderLine> OrderLines { get; set; } = new List<OrderLine>();
    }
}
﻿using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RMSAutoAPI.Models
{
    public class OrderHead
    {
        [Description("Описание")]
        public string CustOrderNum { get; set; }
        [Description("Еще одно описание")]
        public string OrderNotes { get; set; }
        [Description("реакция на частичное размещение. " +
                    "\n1 - разместить все что разместилось (статусы строк не проверяются, кол-во при надобности уменьшается до возможного или выравнивается по MinQty, цена не проверяется)" +
                    "\n0 - в случае любой коллизии заказ не размещать (статусы строк не проверяются)," +
                    "\n2 - статусы строк размещения рассматриваются по отдельности")]
        [Required]
        public Reaction ValidationType { get; set; }

        [Required]
        [Description("false - размещать заказ. true - не размещать заказ")]
        public bool IsTest { get; set; } = true;

        [Required]
        [Description("Детальки")]
        public List<OrderHeadLine> OrderHeadLines { get; set; }
    }
}
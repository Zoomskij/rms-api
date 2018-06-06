using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RMSAutoAPI.Models
{
    public class OrderHead
    {
        [Description("Клиентский номер заказа")]
        public string CustOrderNum { get; set; }
        [Description("Примечание к заказу")]
        public string OrderNotes { get; set; }
        [Description("Реакция на возникшие коллизии:" + 
			"\n0 - в случае любой коллизии заказ не размещать;" +
			"\n1 - разместить все позиции, кроме отсутствующих в остатках. При этом цена всегда наша текущая, а кол-во при нехватке уменьшается до имеющегося и выравнивается по кратности (поле MinOrderQty в результатах поиска это и есть кратность или 'минимальное количество для заказа');" +
			"\n2 - коллизии обрабатываются построчно (в каждой строке задаются свои реакции)")]
        [Required]
        public Reaction ValidationType { get; set; }
        [Required]
        [Description("true - тестовый заказ, не попадает в учётную систему и имеет время жизни ~24ч. (значение по умолчанию)" +
			"\nfalse - реальный заказ")]
        public bool IsTest { get; set; } = true;
        [Required]
        [Description("Строки заказа")]
        public List<OrderHeadLine> OrderHeadLines { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace RMSAutoAPI.Models
{
    public class Order<T>
    {
        public int OrderId { get; set; }
        //[JsonIgnore]
        //[ScriptIgnore]
        [Description("наименование заказа (необязательное поле)")]
        public string OrderName { get; set; }
        [Description("реакция на частичное размещение. " +
            "\n1 - разместить все что разместилось (статусы строк не проверяются, кол-во при надобности уменьшается до возможного или выравнивается по MinQty, цена не проверяется)" +
            "\n0 - в случае любой коллизии заказ не размещать (статусы строк не проверяются)," +
            "\n2 - статусы строк размещения рассматриваются по отдельности")]
        public Reaction Reaction { get; set; }
        [Description("Имя пользователя")]
        public string Username { get; set; }
        [Description("Дата размещения")]
        public DateTime OrderDate { get; set; }
        [Description("Дата завершения")]
        public DateTime? CompletedDate { get; set; }
        [Description("Статус заказа")]
        public byte Status { get; set; }
        [Description("Сумма заказа")]
        public decimal Total { get; set; }
        [Description("Детали заказа")]
        public List<T> SpareParts { get; set; } = new List<T>();
    }
}
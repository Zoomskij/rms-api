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
        [Description("Статус размещения:" +
			"\n0 - Строка успешна размещена" +
			"\n1 - Строка успешно размещена. Кол-во снижено по имеющегося остатка." +
			"\n2 - Строка успешно размещена. Кол-во выровнено по кратности вверх." +
			"\n3 - Строка успешно размещена. Кол-во выровнено по кратности вниз." +
			"\n10 - Строка не размещена. Не указано одно из ключевых значений: Brand, Article, SupplierID" +
			"\n20 - Строка не размещена. Не указано кол-во (Count)." +
			"\n30 - Строка не размещена. Не указан признак разрешения замены (StrictlyThisNumber)." +
			"\n40 - Строка не размещена. Не указана реакция на коллизию по количеству (ReactionByCount)." +
			"\n41 - Строка не размещена. Коллизия по количеству." +
			"\n50 - Строка не размещена. Не указана реакция на коллизию по цене (ReactionByPrice)." +
			"\n51 - Строка не размещена. Коллизия по цене." +
			"\n99 - Строка не размещена. Позиция не найдена в остатках.")]
        public ResponsePartNumber Status { get; set; }
        [Description("Код позиции")]
        public string Reference { get; set; }
    }
}
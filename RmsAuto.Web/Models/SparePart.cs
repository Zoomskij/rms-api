using System.ComponentModel;

namespace RMSAutoAPI.Models
{
    public class SparePart : Part
    {
        [Description("Описание артикула")]
        public string Name { get; set; } = string.Empty;
        [Description("Тип артикула (0 - точное совпадение; 1 - переход; 2 - аналог)")]
        public SparePartItemType Type { get; set; }
        [Description("Качество поставки (1 - отличное; 2 - среднее; 3 - посредственное; 0 - нет информации)")]
        public Quality DeliveryQuality { get; set; }
        [Description("Количество, доступное к заказу на складе поставщика")]
        public int? Count { get; set; }
        [Description("Минимальный срок поставки")]
        public int DeliveryDaysMin { get; set; }
        [Description("Максимальный срок поставки")]
        public int DeliveryDaysMax { get; set; }
        [Description("Цена")]
        public decimal Price { get; set; }
        [Description("Минимальное количество для заказа")]
        public int? MinOrderQty { get; set; }
    }
}
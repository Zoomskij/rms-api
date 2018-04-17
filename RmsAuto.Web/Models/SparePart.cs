using System.ComponentModel;

namespace RMSAutoAPI.Models
{
    public class SparePart : Part
    {
        /// <summary>
        /// Описание артикула
        /// </summary>
        [Description("Описание артикула")]
        public string Name { get; set; }
        /// <summary>
        /// Тип артикула (0 - точное совпадение; 1 - переход; 2 - аналог)
        /// </summary>
        [Description("Тип артикула (0 - точное совпадение; 1 - переход; 2 - аналог)")]
        public SparePartItemType Type { get; set; }
        /// <summary>
        /// Качество поставки (1 - отличное; 2 - среднее; 3 - посредственное; 0 - нет информации)
        /// </summary>
        [Description("Качество поставки (1 - отличное; 2 - среднее; 3 - посредственное; 0 - нет информации)")]
        public Quality DeliveryQuality { get; set; }
        /// <summary>
        /// Количество, доступное к заказу на складе поставщика
        /// </summary>
        [Description("Количество, доступное к заказу на складе поставщика")]
        public int? Count { get; set; }
        /// <summary>
        /// Минимальный срок поставки
        /// </summary>
        [Description("Минимальный срок поставки")]
        public int DeliveryDaysMin { get; set; }
        /// <summary>
        /// Максимальный срок поставки
        /// </summary>
        [Description("Максимальный срок поставки")]
        public int DeliveryDaysMax { get; set; }
        /// <summary>
        /// Цена
        /// </summary>
        [Description("Цена")]
        public decimal Price { get; set; }
        /// <summary>
        /// Минимальное количество для заказа
        /// </summary>
        [Description("Минимальное количество для заказа")]
        public int? MinOrderQty { get; set; }
    }
}
namespace RMSAutoAPI.Models
{
    public class PartNumber
    {
        /// <summary>
        /// Бренд
        /// </summary>
        public string Brand { get; set; }
        /// <summary>
        /// Артикул
        /// </summary>
        public string Article { get; set; }
        /// <summary>
        /// Описание артикула
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Тип артикула (0 - точное совпадение; 1 - переход; 2 - аналог)
        /// </summary>
        public SparePartItemType Type { get; set; }
        /// <summary>
        /// Качество поставки (1 - отличное; 2 - среднее; 3 - посредственное; 0 - нет информации)
        /// </summary>
        public Quality DeliveryQuality { get; set; }
        /// <summary>
        /// Количество, доступное к заказу на складе поставщика
        /// </summary>
        public int? Count { get; set; }
        /// <summary>
        /// Минимальный срок поставки
        /// </summary>
        public int DeliveryDaysMin { get; set; }
        /// <summary>
        /// Максимальный срок поставки
        /// </summary>
        public int DeliveryDaysMax { get; set; }
        /// <summary>
        /// Цена
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// Код поставщика
        /// </summary>
        public int SupplierID { get; set; }
        /// <summary>
        /// Минимальное количество для заказа
        /// </summary>
        public int? MinOrderQty { get; set; }
    }
}
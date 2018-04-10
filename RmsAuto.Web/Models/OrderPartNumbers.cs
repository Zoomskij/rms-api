namespace RMSAutoAPI.Models
{
    public class OrderPartNumbers : Part 
    {
        /// <summary>
        /// Количество
        /// </summary>
        public int? Count { get; set; }
        /// <summary>
        /// Цена
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// Reference (необязательное поле)
        /// </summary>
        public byte Reference { get; set; }
        /// <summary>
        /// - признак разрешения замены у поставщика (обязательное поле)
        /// 0 - только заказанный номер
        /// 1 - разрешена замена от поставщика
        /// </summary>
        public byte ReacionByChange { get; set; } = 0;
        /// <summary>
        /// - признак реакции на коллизию кол-ва
        /// 0 - только указанное кол-во
        /// 1 - брать сколько есть, но не выше указанного
        /// 2 - разрешение при необходимости на выравнивание вверх по MinQty
        /// </summary>
        public byte ReactionByCount { get; set; } = 0;
        /// <summary>
        /// - признак реакции на коллизию цены
        /// 0 - не выше указанной цены
        /// 1 - текущая цена поставщика(т.е.без проверки цены из заказа не зависимо от ее указания в заказе)
        /// </summary>
        public byte ReactionByPrice { get; set; } = 0;
        /// <summary>
        /// признак реакции на выбор поставщика
        /// 0 - только указанный поставщик
        /// 1 - (зарезервировано для автоматического подбора поставщика по цене)
        /// 2 - (зарезервировано для автоматического подбора поставщика по сроку)
        /// 3 - (зарезервировано для автоматического подбора поставщика по качеству поставки)
        /// </summary>
        public byte ReactionBySupplier { get; set; } = 0;

    }
}
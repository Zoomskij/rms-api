using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RMSAutoAPI.Models
{
    public class OrderHeadLine : Part
    {
        [Required]
        [Description("Количество")]
        public int Count { get; set; }
        [Description("Цена")]
        public decimal Price { get; set; }
        [Description("Код позиции")]
        public string Reference { get; set; } = string.Empty;
        [Description("Реакция на коллизию по количеству:" +
			"\n0 - размещаем строго указанное в поле Count кол-во, иначе позиция отбрасывается" +
            "\n1 - размещаем столько, сколько есть в остатках, но не больше указанного в поле Count кол-ва, но при несоответствии по кратности разрешаем выравнивание вверх по кратности (МinOrderQty объекта SparePart)" +
            "\n2 - размещаем столько, сколько есть в остатках, но не больше указанного в поле Count кол-ва, но при несоответствии по кратности разрешаем выравнивание вниз по кратности (МinOrderQty объекта SparePart)")]
        public byte ReactionByCount { get; set; }
        [Description("Реакция на коллизию по цене:" +
			"\n0 - размещаем не выше указанной в поле Price цены, иначе позиция отбрасывается" +
			"\n1 - размещаем по нашей текущей цене (независимо от цены в поле Price)")]
        public byte ReactionByPrice { get; set; }
        [Description("Строго этот номер (замены неприемлемы)")]
        public bool StrictlyThisNumber { get; set; }
    }
}
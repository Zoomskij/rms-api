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
        [Description("Описание")]
        public string Reference { get; set; } = string.Empty;
        [Description("Реакцию на коллизию по количеству")]
        public byte ReactionByCount { get; set; }
        [Description("Реакцию на коллизию по цене")]
        public byte ReactionByPrice { get; set; }
        [Description("Строго этот номер")]
        public bool StrictlyThisNumber { get; set; }
    }
}
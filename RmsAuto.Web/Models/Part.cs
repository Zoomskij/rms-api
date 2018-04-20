using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RMSAutoAPI.Models
{
    public class Part
    {
        [Required]
        [Description("Бренд")]
        public string Brand { get; set; } = string.Empty;
        [Required]
        [Description("Артикул")]
        public string Article { get; set; } = string.Empty;
        [Required]
        [Description("Код поставщика")]
        public int SupplierID { get; set; }

    }
}
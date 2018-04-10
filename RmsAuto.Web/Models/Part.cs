using System.ComponentModel;
namespace RMSAutoAPI.Models
{
    public class Part
    {
        [Description("Бренд")]
        public string Brand { get; set; }
        [Description("Артикул")]
        public string Article { get; set; }
        [Description("Код поставщика")]
        public int SupplierID { get; set; }

    }
}
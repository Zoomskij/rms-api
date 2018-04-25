using System.ComponentModel;

namespace RMSAutoAPI.Models
{
    public class Brand
    {
        [Description("Наименование производителя")]
        public string Name { get; set; } = string.Empty;
        [Description("Описание детали")]
        public string Description { get; set; } = string.Empty;
    }
}

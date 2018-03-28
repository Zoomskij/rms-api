using System.ComponentModel;

namespace RMSAutoAPI.Models
{
    public class Brand
    {
        /// <summary>
        /// Наименование производителя
        /// </summary>
        [Description("Наименование производителя")]
        public string Name { get; set; }
        /// <summary>
        /// Описание детали
        /// </summary>
        [Description("Описание детали")]
        public string Description { get; set; }
    }
}

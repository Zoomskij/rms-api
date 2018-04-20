using System.ComponentModel;

namespace RMSAutoAPI.Models
{
    public class Partner
    {

        /// <summary>
        /// Город
        /// </summary>
        [Description("Город")]
        public string City { get; set; } = string.Empty;
        /// <summary>
        /// Необходимый Параметр
        /// </summary>
        [Description("Код партнёра - необходимый параметр для авторизации региональных клиентов")]
        public string Code { get; set; } = string.Empty;
        /// <summary>
        /// Наименование Партнера
        /// </summary>
        [Description("Наименование партнёра")]
        public string Name { get; set; } = string.Empty;
    }
}
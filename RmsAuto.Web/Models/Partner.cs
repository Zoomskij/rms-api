using System.ComponentModel;

namespace RMSAutoAPI.Models
{
    public class Partner
    {
        [Description("Город")]
        public string City { get; set; } = string.Empty;
        [Description("Код партнёра - необходимый параметр для авторизации региональных клиентов")]
        public string Code { get; set; } = string.Empty;
        [Description("Наименование партнёра")]
        public string Name { get; set; } = string.Empty;
    }
}
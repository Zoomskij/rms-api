using System.ComponentModel;

namespace RMSAutoAPI.Models
{
    public class Partner
    {

        /// <summary>
        /// Город
        /// </summary>
        [Description("Город")]
        public string City { get; set; }
        /// <summary>
        /// Необходимый Параметр
        /// </summary>
        [Description("Необходимый Параметр")]
        public string InternalFranchName { get; set; }
        /// <summary>
        /// Наименование Партнера
        /// </summary>
        [Description("Наименование Партнера")]
        public string Franch { get; set; }
    }
}
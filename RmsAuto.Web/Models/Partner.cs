using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RMSAutoAPI.Models
{
    public class Partner
    {

        /// <summary>
        /// Город
        /// </summary>
        public string City { get; set; }
        /// <summary>
        /// Необходимый Параметр
        /// </summary>
        public string InternalFranchName { get; set; }
        /// <summary>
        /// Наименование Партнера
        /// </summary>
        public string Franch { get; set; }
    }
}
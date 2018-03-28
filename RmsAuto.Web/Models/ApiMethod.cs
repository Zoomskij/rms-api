using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RMSAutoAPI.Models
{
    public class ApiMethod
    {
        public string Group { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string Uri { get; set; }
        public string Description { get; set; }
        public string TitleDescription { get; set; }
        public List<ApiParameter> Parameters { get; set; } = new List<ApiParameter>();
        public object Response { get; set; }
    }
}
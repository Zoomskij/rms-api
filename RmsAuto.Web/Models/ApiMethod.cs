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
        public List<ApiResponse> Responses { get; set; } = new List<ApiResponse>();
        public List<ApiPermissions> Permissions { get; set; } = new List<ApiPermissions>();
        public object Response { get; set; }
        public string ResponseName  => Response.GetType().Name.Replace("`1", string.Empty);
        public bool Visible { get; set; }
        public bool AllowAnonymous { get; set; }
    }
}
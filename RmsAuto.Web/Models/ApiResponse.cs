using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RMSAutoAPI.Models
{
    public class ApiResponse
    {
        public int Id { get; set; }
        public int MethodId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}

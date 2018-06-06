using System.Collections.Generic;

namespace RMSAutoAPI.Models
{
    public class Model
    {
        public string Name { get; set; }
        public List<Parameter> Parameters { get; set; } = new List<Parameter>();
    }
}
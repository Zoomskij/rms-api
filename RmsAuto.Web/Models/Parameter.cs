using Newtonsoft.Json;
using System.Web.Script.Serialization;

namespace RMSAutoAPI.Models
{
    public class Parameter
    {
        public string Name { get; set; }
        public string Type { get; set; }

        [JsonIgnore]
        //[ScriptIgnore]
        public string Description { get; set; }
    }
}
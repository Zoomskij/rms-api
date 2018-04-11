using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace RMSAutoAPI.Models
{
    public class Order<T>
    {
        public int OrderId { get; set; }
        //[JsonIgnore]
        //[ScriptIgnore]
        public string OrderName { get; set; }
        public Reaction Reaction { get; set; }
        public string Username { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public byte Status { get; set; }
        public decimal Total { get; set; }
        public List<T> PartNumbers { get; set; } = new List<T>();
    }
}
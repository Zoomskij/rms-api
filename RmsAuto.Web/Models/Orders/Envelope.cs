using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace RMSAutoAPI.Models.Orders
{
    public class Envelope
    {
        public static Envelope Empty()
        {
            return new Envelope();
        }

        [XmlAttribute("Name")]
        public string Action { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace RMSAutoAPI.Models.Orders
{
    public class OrderHandlingInfo
    {
        [XmlElement("line")]
        public OrderLineHandlingInfo[] LineHandlings { get; set; }
    }
}
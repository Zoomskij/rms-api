using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace RMSAutoAPI.Models.Orders
{
    public class OrderLineHandlingInfo
    {
        [XmlElement("OrderLineWeb")]
        public int WebOrderLineId { get; set; }

        [XmlElement("OrderLineHW")]
        public int AcctgOrderLineId { get; set; }

        [XmlElement("LineStatus")]
        public string OrderLineStatus { get; set; }
    }
}
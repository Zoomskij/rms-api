using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace RMSAutoAPI.Models.Orders
{
    public class SendOrderEnvelope : Envelope
    {
        [XmlElement("ClientCode")]
        public string ClientId { get; set; }

        public OrderInfo Order { get; set; }
    }
}
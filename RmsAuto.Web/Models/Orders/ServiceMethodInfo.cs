using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace RMSAutoAPI.Models.Orders
{
    class ServiceMethodInfo
    {
        public string MethodName { get; set; }
        public string ServiceAction { get; set; }
        public XmlSerializer ArgsSerializer { get; set; }
        public XmlSerializer ResultsSerializer { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RMSAutoAPI.Models.Orders
{
    public class SendOrderResultsEnvelope : Envelope
    {
        public OrderHandlingInfo Order { get; set; }
    }
}
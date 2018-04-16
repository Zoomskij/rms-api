using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace RMSAutoAPI.Models.Orders
{
    class CustomStringWriter : StringWriter
    {
        private Encoding _encoding;

        public CustomStringWriter(Encoding encoding)
        {
            if (encoding == null)
                throw new ArgumentNullException("encoding");
            _encoding = encoding;
        }

        public override Encoding Encoding
        {
            get { return _encoding; }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RMSAutoAPI.Models.Orders
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    class ServiceMethodAttribute : Attribute
    {
        private string _action;
        private Type _argType;
        private Type _returnType;

        public ServiceMethodAttribute(string action, Type argType, Type returnType)
        {
            if (string.IsNullOrEmpty(action))
                throw new ArgumentException("ServiceMethod action cannot be empty", "action");
            if (argType == null)
                throw new ArgumentNullException("argType");
            if (returnType == null)
                throw new ArgumentNullException("returnType");
            _action = action;
            _argType = argType;
            _returnType = returnType;
        }

        public string Action
        {
            get { return _action; }
            set { _action = value; }
        }

        public Type ArgType
        {
            get { return _argType; }
            set { _argType = value; }
        }

        public Type ReturnType
        {
            get { return _returnType; }
            set { _returnType = value; }
        }
    }
}
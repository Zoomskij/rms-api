//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace RMSAutoAPI.App_Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class Parameters
    {
        public int Id { get; set; }
        public int MethodId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public int TypeParameter { get; set; }
        public bool IsRequired { get; set; }
    
        public virtual Methods Methods { get; set; }
    }
}

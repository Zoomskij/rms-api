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
    
    public partial class Settings
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int RateId { get; set; }
    
        public virtual Rates Rates { get; set; }
        public virtual Users Users { get; set; }
    }
}

﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class ex_rmsauto_logEntities : DbContext
    {
        public ex_rmsauto_logEntities()
            : base("name=ex_rmsauto_logEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<LogRequests> LogRequests { get; set; }
        public virtual DbSet<SearchSparePartsLog> SearchSparePartsLog { get; set; }
        public virtual DbSet<ErrorLog> ErrorLog { get; set; }
        public virtual DbSet<SearchSparePartsWebServiceLog> SearchSparePartsWebServiceLog { get; set; }
    }
}
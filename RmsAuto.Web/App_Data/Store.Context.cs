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
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class ex_rmsauto_storeEntities : DbContext
    {
        public ex_rmsauto_storeEntities()
            : base("name=ex_rmsauto_storeEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Rates> Rates { get; set; }
        public virtual DbSet<Settings> Settings { get; set; }
        public virtual DbSet<Users> Users { get; set; }
        public virtual DbSet<BrandEquivalents> BrandEquivalents { get; set; }
        public virtual DbSet<LogRequests> LogRequests { get; set; }
        public virtual DbSet<OrderLines> OrderLines { get; set; }
        public virtual DbSet<Orders> Orders { get; set; }
    
        public virtual ObjectResult<spSearchCrossesWithPriceSVC_Result> spSearchCrossesWithPriceSVC(string partNumber, string manufacturer, Nullable<bool> showAnalogs, string selectedBrands, string acctgID, Nullable<int> clientGroup, string region)
        {
            var partNumberParameter = partNumber != null ?
                new ObjectParameter("PartNumber", partNumber) :
                new ObjectParameter("PartNumber", typeof(string));
    
            var manufacturerParameter = manufacturer != null ?
                new ObjectParameter("Manufacturer", manufacturer) :
                new ObjectParameter("Manufacturer", typeof(string));
    
            var showAnalogsParameter = showAnalogs.HasValue ?
                new ObjectParameter("ShowAnalogs", showAnalogs) :
                new ObjectParameter("ShowAnalogs", typeof(bool));
    
            var selectedBrandsParameter = selectedBrands != null ?
                new ObjectParameter("SelectedBrands", selectedBrands) :
                new ObjectParameter("SelectedBrands", typeof(string));
    
            var acctgIDParameter = acctgID != null ?
                new ObjectParameter("AcctgID", acctgID) :
                new ObjectParameter("AcctgID", typeof(string));
    
            var clientGroupParameter = clientGroup.HasValue ?
                new ObjectParameter("ClientGroup", clientGroup) :
                new ObjectParameter("ClientGroup", typeof(int));
    
            var regionParameter = region != null ?
                new ObjectParameter("Region", region) :
                new ObjectParameter("Region", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<spSearchCrossesWithPriceSVC_Result>("spSearchCrossesWithPriceSVC", partNumberParameter, manufacturerParameter, showAnalogsParameter, selectedBrandsParameter, acctgIDParameter, clientGroupParameter, regionParameter);
        }
    
        public virtual ObjectResult<spSearchBrands_Result> spSearchBrands(string partNumber, Nullable<bool> showAnalogs, string acctgID, string region)
        {
            var partNumberParameter = partNumber != null ?
                new ObjectParameter("PartNumber", partNumber) :
                new ObjectParameter("PartNumber", typeof(string));
    
            var showAnalogsParameter = showAnalogs.HasValue ?
                new ObjectParameter("ShowAnalogs", showAnalogs) :
                new ObjectParameter("ShowAnalogs", typeof(bool));
    
            var acctgIDParameter = acctgID != null ?
                new ObjectParameter("AcctgID", acctgID) :
                new ObjectParameter("AcctgID", typeof(string));
    
            var regionParameter = region != null ?
                new ObjectParameter("Region", region) :
                new ObjectParameter("Region", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<spSearchBrands_Result>("spSearchBrands", partNumberParameter, showAnalogsParameter, acctgIDParameter, regionParameter);
        }
    
        public virtual ObjectResult<spGetFranches_Result> spGetFranches()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<spGetFranches_Result>("spGetFranches");
        }
    }
}

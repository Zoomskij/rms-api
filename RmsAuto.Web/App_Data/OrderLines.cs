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
    
    public partial class OrderLines
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public OrderLines()
        {
            this.OrderLineStatusChanges = new HashSet<OrderLineStatusChanges>();
        }
    
        public int OrderLineID { get; set; }
        public Nullable<int> AcctgOrderLineID { get; set; }
        public Nullable<int> ParentOrderLineID { get; set; }
        public int OrderID { get; set; }
        public string PartNumber { get; set; }
        public string Manufacturer { get; set; }
        public int SupplierID { get; set; }
        public int DeliveryDaysMin { get; set; }
        public int DeliveryDaysMax { get; set; }
        public string PartName { get; set; }
        public string PartDescription { get; set; }
        public Nullable<decimal> WeightPhysical { get; set; }
        public Nullable<decimal> WeightVolume { get; set; }
        public decimal UnitPrice { get; set; }
        public int Qty { get; set; }
        public bool StrictlyThisNumber { get; set; }
        public string VinCheckupData { get; set; }
        public string OrderLineNotes { get; set; }
        public Nullable<System.DateTime> EstSupplyDate { get; set; }
        public byte CurrentStatus { get; set; }
        public Nullable<System.DateTime> CurrentStatusDate { get; set; }
        public string ReferenceID { get; set; }
        public byte Processed { get; set; }
        public Nullable<decimal> SupplierPriceWithMarkup { get; set; }
        public Nullable<byte> ItemDeliveryType { get; set; }
    
        public virtual Orders Orders { get; set; }
        public virtual OrderLineStatuses OrderLineStatuses { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderLineStatusChanges> OrderLineStatusChanges { get; set; }
    }
}

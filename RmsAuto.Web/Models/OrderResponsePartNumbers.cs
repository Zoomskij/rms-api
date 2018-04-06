namespace RMSAutoAPI.Models
{
    public class OrderResponsePartNumbers
    {
        public int SupplierID { get; set; }
        public string Brand { get; set; }
        public string Article { get; set; }
        public int CountOrder { get; set; }
        public int CountApproved { get; set; }
        public decimal PriceOrder { get; set; }
        public decimal PriceApproved { get; set; }
        public ResponsePartNumber Status { get; set; }
    }
}
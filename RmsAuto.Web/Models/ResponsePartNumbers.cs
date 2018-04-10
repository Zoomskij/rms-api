namespace RMSAutoAPI.Models
{
    public class ResponsePartNumbers : Part
    {
        public int CountOrder { get; set; }
        public int CountApproved { get; set; }
        public decimal PriceOrder { get; set; }
        public decimal PriceApproved { get; set; }
        public ResponsePartNumber Status { get; set; }
    }
}
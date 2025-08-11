namespace StockNotificationWarning.Dto
{
    public class ProductWithVendorDto
    {
        public string? ProductId { get; set; }
        public string? ProductTitle { get; set; }
        public Vendor? Vendor { get; set; }
    }
}

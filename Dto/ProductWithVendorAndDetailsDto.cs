using StockNotificationWarning.Entities;

namespace StockNotificationWarning.Dto
{
    public class ProductWithVendorAndDetailsDto
    {
        public string? ProductId { get; set; }
        public string? ProductTitle { get; set; }
        public Vendor? Vendor { get; set; }
        public List<ProductDetails> Details { get; set; } = [];
    }
}

using System.Text.Json;

namespace StockNotificationWarning.Dto
{
    public class Vendor
    {
        public long? Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? WebsiteUrl { get; set; }
    }
}

using System.Text.Json.Serialization;

namespace StockNotificationWarning.Dto
{
    public class MetafieldAccess
    {
        [JsonPropertyName("admin")]
        public string? Admin { get; set; }
        [JsonPropertyName("storefront")]
        public string? Storefront { get; set; }
    }
}

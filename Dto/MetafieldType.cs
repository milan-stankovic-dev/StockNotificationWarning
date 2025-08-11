using System.Text.Json.Serialization;

namespace StockNotificationWarning.Dto
{
    public class MetafieldType
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }
    }
}

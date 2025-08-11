using System.Text.Json.Serialization;

namespace StockNotificationWarning.Dto
{
    public class MetafieldCreatedDefinition
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }
        [JsonPropertyName("name")]
        public string? Name { get; set; }
        [JsonPropertyName("namespace")]
        public string? Namespace { get; set; }
        [JsonPropertyName("key")]
        public string? Key { get; set; }
        [JsonPropertyName("type")]
        public MetafieldType? Type { get; set; }
        [JsonPropertyName("access")]
        public MetafieldAccess? Access { get; set; }
    }
}

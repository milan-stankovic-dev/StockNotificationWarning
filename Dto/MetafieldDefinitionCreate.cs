using System.Text.Json.Serialization;

namespace StockNotificationWarning.Dto
{
    public class MetafieldDefinitionCreate
    {
        [JsonPropertyName("metafieldDefinitionCreate")]
        public MetafieldCreatedDefinition? Payload { get; set; }
    }
}

using System.Text.Json.Serialization;

namespace StockNotificationWarning.Dto
{
    public class MetafieldDefinitionCreateResponse
    {
        [JsonPropertyName("metafieldDefinitionCreate")]
        public MetafieldDefinitionCreatePayload? MetafieldDefinitionCreate { get; set; }
    }
}

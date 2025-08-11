using System.Text.Json.Serialization;

namespace StockNotificationWarning.Dto
{
    public class MetafieldDefinitionsResponse
    {
        [JsonPropertyName("metafieldDefinitions")]
        public MetafieldDefinitionsConnection? MetafieldDefinitions { get; set; }
    }
}

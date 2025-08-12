using System.Text.Json.Serialization;

namespace StockNotificationWarning.Dto
{
    public class MetafieldEdge
    {
        [JsonPropertyName("node")]
        public MetafieldCreatedDefinition? Node { get; set; }
    }
}

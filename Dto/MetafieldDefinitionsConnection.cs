using System.Text.Json.Serialization;

namespace StockNotificationWarning.Dto
{
    public class MetafieldDefinitionsConnection
    {
        [JsonPropertyName("edges")]
        public List<MetafieldEdge> Edges { get; set; } = [];
    }
}

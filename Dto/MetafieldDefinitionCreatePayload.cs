using System.Text.Json.Serialization;

namespace StockNotificationWarning.Dto
{
    public class MetafieldDefinitionCreatePayload
    {
        [JsonPropertyName("createdDefinition")]
        public MetafieldCreatedDefinition? CreatedDefinition { get; set; }
        [JsonPropertyName("userErrors")]
        public List<UserError> UserErrors { get; set; } = [];
    }
}

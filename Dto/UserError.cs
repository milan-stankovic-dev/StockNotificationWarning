using System.Text.Json.Serialization;

namespace StockNotificationWarning.Dto
{
    public class UserError
    {
        [JsonPropertyName("field")]
        public string? Field { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }

        [JsonPropertyName("code")]
        public string? Code { get; set; }
    }
}

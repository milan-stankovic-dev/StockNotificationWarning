using System.Text.Json.Serialization;

namespace StockNotificationWarning.Dto
{
    public class GQLGenericResponse<T>
    {
        [JsonPropertyName("data")]
        public T? Data { get; set; }
    }
}

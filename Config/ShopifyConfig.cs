namespace StockNotificationWarning.Config
{
    public class ShopifyConfig
    {
        public string ApiKey { get; init; } = default!;
        public string SecretKey { get; init; } = default!;
        public string Scopes { get; init; } = default!;
        public string RedirectUri { get; init; } = default!;
    }
}

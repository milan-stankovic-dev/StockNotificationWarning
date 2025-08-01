using StockNotificationWarning.Services.Abstraction;

namespace StockNotificationWarning.Services
{
    public class ShopifyCredentialStore(ILogger<ShopifyCredentialStore> logger) : IShopifyCredentialStore
    {
        readonly ILogger<ShopifyCredentialStore> _logger = logger;
        public string? Shop { get; private set; }

        readonly Dictionary<string, string> _store = new();

        public string? AccessToken { get; private set; }

        public void InitializeAsync(HttpContext context,
                                          string? shop = null,
                                          string? token = null)
        {
            Shop = shop ?? context.Request.Query["shop"];
            AccessToken = token ?? context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            _logger.LogInformation("INIT ASYNC CALLED FOR SHOPIFY CONTEXT");
        }

        public void Set(HttpContext context, string? shopKey = null, string? tokenValue = null)
        {
            shopKey = shopKey ?? context.Request.Query["shop"];
            tokenValue = tokenValue ?? context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            _store[shopKey ?? "N/A"] = tokenValue;
        }

        public string? Get(string shopKey)
        {
            _store.TryGetValue(shopKey, out var value);
            return value;
        }
        
    }
}

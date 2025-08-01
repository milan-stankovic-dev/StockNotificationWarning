using StockNotificationWarning.Services.Abstraction;

namespace StockNotificationWarning.Services
{
    public class ShopifyCredentialStore : IShopifyCredentialStore
    {
        readonly Dictionary<string, string> _store = new();

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

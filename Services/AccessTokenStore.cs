using StockNotificationWarning.Services.Abstraction;

namespace StockNotificationWarning.Services
{
    public class AccessTokenStore : IAccessTokenStore
    {
        readonly Dictionary<string, string> _store = new();
        public string? Get(string shop) => 
            _store.TryGetValue(shop, out var value) ? value : null;

        public void Set(string shop, string token) =>
            _store["shop"] = token;
    }
}

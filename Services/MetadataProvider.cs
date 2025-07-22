using StockNotificationWarning.Config;
using StockNotificationWarning.Services.Abstraction;

namespace StockNotificationWarning.Services
{
    public class MetadataProvider(IHttpContextAccessor contextAccessor) : IMetadataProvider
    {
        readonly IHttpContextAccessor _contextAccessor = contextAccessor;
        public async Task<Dictionary<string, string>> Provide() => await Provide(null);

        public Task<Dictionary<string, string>> Provide(string? defaultShopName)
        {
            var context = _contextAccessor.HttpContext;

            var shopName = ShopifySessionStore.ShopName;
            var token = ShopifySessionStore.AccessToken;

            return Task.FromResult(
                new Dictionary<string, string>
                {
                    { "shopName", shopName },
                    { "accessToken", token }
                }
            );
        }
    }
}

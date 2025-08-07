using StockNotificationWarning.Services.Abstraction;

namespace StockNotificationWarning.Services
{
    public class ShopTokenProvider(IConfigDefaultsProvider configDefaults,
                                   IShopifyCredentialStore credentialStore) : IShopTokenProvider
    {
        readonly IConfigDefaultsProvider _configDefaults = configDefaults;
        readonly IShopifyCredentialStore _credentialStore = credentialStore;
        public (string shop, string token) Provide()
        {
            var shop = _configDefaults.Provide();
            var token = _credentialStore.Get(shop) ?? "N/A";

            return (shop, token);
        }

    }
}

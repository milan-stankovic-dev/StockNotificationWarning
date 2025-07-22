using Microsoft.Extensions.Options;
using StockNotificationWarning.Defaults;
using StockNotificationWarning.Services.Abstraction;

namespace StockNotificationWarning.Services
{
    public class ConfigDefaultsProvider(IOptions<ShopDefaults> options) : IConfigDefaultsProvider
    {
        readonly string _storeName = options.Value.ShopName;
        public string Provide() => _storeName;
    }
}

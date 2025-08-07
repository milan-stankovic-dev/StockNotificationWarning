using ShopifySharp;
using StockNotificationWarning.Services.Abstraction;

namespace StockNotificationWarning.Services
{
    public class ShopifyScopeService(IShopTokenProvider shopTokenProvider,
                                    ILogger<ShopifyScopeService> logger) : IShopifyScopeService
    {
        readonly IShopTokenProvider _shopTokenProvider = shopTokenProvider;
        readonly ILogger<ShopifyScopeService> _logger = logger;

        public async Task<string> GetAllScopes()
        {
            (string shop, string token) = _shopTokenProvider.Provide();
            var gql = new GraphService(shop, token);

            var query = @"{
                currentAppInstallation {
                    accessScopes {
                        handle
                    }
                }
            }";

            var response = await gql.PostAsync(query);
            _logger.LogInformation($"SCOPES RESPONSE: {response}");
            return response.Root.ToString();
        }
    }
}

using ShopifySharp;
using StockNotificationWarning.Services.Abstraction;
using System.Text.Json;

namespace StockNotificationWarning.Services
{
    public class ShopifyScopeService(IShopTokenProvider shopTokenProvider) : IShopifyScopeService
    {
        readonly IShopTokenProvider _shopTokenProvider = shopTokenProvider;

        public async Task<string[]> GetAllScopes()
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

            var response = await gql.PostAsync(new GraphRequest
            {
                Query = query
            });

            return FormatScopes(response.Json);
        }

        private string[] FormatScopes(ShopifySharp.Infrastructure.Serialization.Json.IJsonElement jsonElement)
        {
            if(jsonElement is null)
            {
                throw new Exception("Could not format scopes " +
                    "due to null value (jsonElement is null)");
            }

            using var doc = JsonDocument.Parse(jsonElement.GetRawText()!);
            var root = doc.RootElement;

            return [.. root
                .GetProperty("data")
                .GetProperty("currentAppInstallation")
                .GetProperty("accessScopes")
                .EnumerateArray()
                .Select(scope => scope.GetProperty("handle").GetString()!)];
        }
    }
}

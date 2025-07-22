using Microsoft.Extensions.Options;
using ShopifySharp;
using ShopifySharp.Filters;
using ShopifySharp.Lists;
using StockNotificationWarning.Config;
using StockNotificationWarning.Services.Abstraction;

namespace StockNotificationWarning.Services
{
    public class ShopifyRequestService(IOptionsMonitor<ShopifyConfig> options) : IShopifyRequestService
    {
        private readonly ShopifyConfig _config = options.CurrentValue;
        public async Task ActivateAsync(long productId, string shop, string accessToken)
        {
            var productService = new ProductService(shop, accessToken);
            await productService.UpdateAsync(productId, new Product
            {
                Status = "active"
            });
        }

        public async Task DeleteAsync(long productId, string shop, string accessToken)
        {
            var productService = new ProductService(shop, accessToken);
            await productService.DeleteAsync(productId);
        }

        public async Task DeleteAllWithStatus(string status, string shop, string accessToken)
        {
            var productService = new ProductService(shop, accessToken);
            ListResult<Product> fetchedProducts = await GetProductsStatusAsync(status,
                shop, accessToken);

            foreach (var product in fetchedProducts.Items)
            {
                await productService.DeleteAsync(product!.Id!.Value);
            }
        }

        public async Task ActivateAllAsync(string shop, string accessToken)
        {
            var productService = new ProductService(shop, accessToken);

            var drafts = await GetProductsStatusAsync("draft", shop, accessToken);

            foreach (var draft in drafts.Items)
            {
                if (draft is null)
                {
                    continue;
                }
                await ActivateAsync(draft!.Id!.Value, shop, accessToken);
            }
        }

        public async Task<ListResult<Product>> GetProductsStatusAsync(string status,
                                                            string shop, string accessToken)
        {
            var productService = new ProductService(shop, accessToken);
            var filter = new ProductListFilter
            {
                Status = status, // npr "draft"
                Limit = 100
            };

            return await productService.ListAsync(filter);
        }

        public string BuildAuthorizationUrl(string shop)
        {
            var uri = AuthorizationService.BuildAuthorizationUrl(
                _config.Scopes.Split(','),
                shop,
                _config.ApiKey,
                _config.RedirectUri
                );

            return uri.ToString();
        }

        public async Task<string> AcquireTokenAsync(string shop, string code)
            => await AuthorizationService.Authorize(
                code, shop, _config.ApiKey, _config.SecretKey);

        public async Task<ListResult<Product>> GetProductsAsync(string shop, string accessToken)
        {
            var productService = new ProductService(shop, accessToken);
            var products = await productService.ListAsync();

            return products;
        }


    }
}

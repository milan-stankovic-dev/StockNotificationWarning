using ShopifySharp;
using ShopifySharp.Lists;

namespace StockNotificationWarning.Services.Abstraction
{
    public interface IShopifyRequestService
    {
        Task ActivateAsync(long productId, string shop, string accessToken);
        Task DeleteAsync(long productId, string shop, string accessToken);
        Task DeleteAllWithStatus(string status, string shop, string accessString);
        Task ActivateAllAsync(string shop, string accessString);
        string BuildAuthorizationUrl(string shop);
        Task<string> AcquireTokenAsync(string shop, string code);
    }
}

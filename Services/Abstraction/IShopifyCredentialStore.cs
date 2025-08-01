namespace StockNotificationWarning.Services.Abstraction
{
    public interface IShopifyCredentialStore
    {
        void Set(HttpContext context, string? shopKey = null, string? tokenValue = null);
        string? Get(string shopKey);

        void InitializeAsync(HttpContext context, string? shop = null, string? token = null);
    }
}

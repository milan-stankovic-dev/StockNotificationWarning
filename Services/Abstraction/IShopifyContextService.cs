namespace StockNotificationWarning.Services.Abstraction
{
    public interface IShopifyContextService
    {
        string? Shop { get; }
        string? Host { get; }
        string? AccessToken { get; }

        void InitializeAsync(HttpContext context, string? shop = null, string? host = null, string? token = null);
    }
}

namespace StockNotificationWarning.Services.Abstraction
{
    public interface IShopifyContextService
    {
        string? Shop { get; }
        string? Host { get; }
        string? AccessToken { get; }

        Task InitializeAsync(HttpContext context, string? shop = null, string? host = null);
    }
}

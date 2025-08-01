using StockNotificationWarning.Services.Abstraction;

namespace StockNotificationWarning.Services
{
    public class ShopifyContextService(ILogger<ShopifyContextService> logger) : IShopifyContextService
    {
        readonly ILogger<ShopifyContextService> _logger = logger;
        public string? Shop { get; private set; }

        public string? Host { get; private set; }

        public string? AccessToken { get; private set; }

        public void InitializeAsync(HttpContext context,
                                          string? shop = null,
                                          string? host = null,
                                          string? token = null)
        {
            Shop = shop ?? context.Request.Query["shop"];
            Host = host ?? context.Request.Query["host"];
            AccessToken = token ?? context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            _logger.LogInformation("INIT ASYNC CALLED FOR SHOPIFY CONTEXT");
            _logger.LogInformation($"Shop: {Shop}, Host: {Host}, AccessToken: {AccessToken}");
        }
    }
}

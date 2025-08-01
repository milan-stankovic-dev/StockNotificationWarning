using StockNotificationWarning.Services.Abstraction;

namespace StockNotificationWarning.Services
{
    public class ShopifyContextService : IShopifyContextService
    {
        public string? Shop { get; private set; }

        public string? Host { get; private set; }

        public string? AccessToken { get; private set; }

        public void InitializeAsync(HttpContext context,
                                          string? shop = null,
                                          string? host = null)
        {
            Shop = shop ?? context.Request.Query["shop"];
            Host = host ?? context.Request.Query["host"];

            AccessToken = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        }
    }
}

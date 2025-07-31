using StockNotificationWarning.Services.Abstraction;

namespace StockNotificationWarning.Services
{
    public class ShopifyContextService(IShopifyRequestService requestService) : IShopifyContextService
    {
        readonly IShopifyRequestService _requestService = requestService;
        public string? Shop { get; private set; }

        public string? Host { get; private set; }

        public string? AccessToken { get; private set; }

        public async Task InitializeAsync(HttpContext context,
                                          string? shop = null,
                                          string? host = null)
        {
            Shop = shop ?? context.Request.Query["shop"];
            Host = host ?? context.Request.Query["host"];

            var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if(!string.IsNullOrEmpty(token))
            {
                AccessToken = await _requestService.ValidateSessionTokenAndGetAccessToken(token);
            }
        }
    }
}

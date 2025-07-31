using StockNotificationWarning.Services.Abstraction;

namespace StockNotificationWarning.Services
{
    public class ShopifyContextService(IShopifyRequestService requestService,
                            ILogger<ShopifyContextService> logger) : IShopifyContextService
    {
        readonly IShopifyRequestService _requestService = requestService;
        readonly ILogger<ShopifyContextService> _logger = logger;
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

            _logger.LogInformation($"Shop: {Shop}, Host: {host}");

            if(!string.IsNullOrEmpty(token))
            {
                AccessToken = await _requestService.ValidateSessionTokenAndGetAccessToken(token);
            }
        }
    }
}

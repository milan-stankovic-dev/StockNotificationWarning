using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StockNotificationWarning.Services.Abstraction;
using System.Threading.Tasks;

namespace StockNotificationWarning.Pages
{
    public class IndexModel(IShopifyRequestService shopifyService,
       IConfigDefaultsProvider configProvider,
       IShopifyContextService contextService) : PageModel
    {
        readonly IShopifyRequestService _shopifyService = shopifyService;
        readonly IConfigDefaultsProvider _configProvider = configProvider;
        readonly IShopifyContextService _contextService = contextService;

        public IActionResult OnGet()
        {
            string shop = _contextService.Shop ?? _configProvider.Provide();
            string authUrl = _shopifyService.BuildAuthorizationUrl(shop);
            _contextService.InitializeAsync(HttpContext, shop);

            return Redirect(authUrl);
        }
    }
}

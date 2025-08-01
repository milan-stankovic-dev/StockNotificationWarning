using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StockNotificationWarning.Services.Abstraction;

namespace StockNotificationWarning.Pages
{
    public class IndexModel(IShopifyRequestService shopifyService,
       IConfigDefaultsProvider configProvider,
       IShopifyCredentialStore credentialStore) : PageModel
    {
        readonly IShopifyRequestService _shopifyService = shopifyService;
        readonly IConfigDefaultsProvider _configProvider = configProvider;
        readonly IShopifyCredentialStore _credentialStore = credentialStore;

        public IActionResult OnGet()
        {
            string shop = _credentialStore.Get("shop") ?? _configProvider.Provide();
            string authUrl = _shopifyService.BuildAuthorizationUrl(shop);
            _credentialStore.Set(HttpContext, shop);

            return Redirect(authUrl);
        }
    }
}

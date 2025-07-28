using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StockNotificationWarning.Services.Abstraction;

namespace StockNotificationWarning.Pages
{
    public class IndexModel(IShopifyRequestService shopifyService,
       IConfigDefaultsProvider configProvider) : PageModel
    {
        readonly IShopifyRequestService _shopifyService = shopifyService;
        readonly IConfigDefaultsProvider _configProvider = configProvider;

        public IActionResult OnGet()
        {
            string shop = _configProvider.Provide();
            string host = Request.Query["host"].ToString();

            if(string.IsNullOrEmpty(host))
            {
                return Content("Host name is missing");
            }

            string authUrl = _shopifyService.BuildAuthorizationUrl(shop);

            return Redirect(authUrl);
        }
    }
}

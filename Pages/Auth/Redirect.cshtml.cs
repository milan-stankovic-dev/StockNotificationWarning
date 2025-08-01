using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StockNotificationWarning.Services.Abstraction;

namespace StockNotificationWarning.Pages.Auth
{
    public class RedirectModel(IShopifyRequestService shopify,
                               IShopifyContextService shopifyContext,
                               IAccessTokenStore store) : PageModel
    {
        readonly IShopifyRequestService _shopify = shopify;
        readonly IShopifyContextService _context = shopifyContext;
        readonly IAccessTokenStore _store = store;

        public async Task<IActionResult> OnGetAsync(string code, string shop, string host)
        {
            await _context.InitializeAsync(HttpContext, shop, host);

            string? token = _context.AccessToken;

            if (string.IsNullOrEmpty(token) || "N/A".Equals(token))
            {
                token = await _shopify.AcquireTokenAsync(shop, code);
            }

            await _shopify.RegisterScriptTagAsync(shop, token);

            return RedirectToPage("/Greeting/HelloWorld", new { host, shop });
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StockNotificationWarning.Services.Abstraction;

namespace StockNotificationWarning.Pages.Auth
{
    public class RedirectModel(IShopifyRequestService shopify,
                               IShopifyCredentialStore credentialStore) : PageModel
    {
        readonly IShopifyRequestService _shopify = shopify;
        readonly IShopifyCredentialStore _credentialStore = credentialStore;

        public async Task<IActionResult> OnGetAsync(string code, string shop, string host)
        {
            string? token = _credentialStore.Get(shop);

            if (string.IsNullOrEmpty(token) || "N/A".Equals(token))
            {
                token = await _shopify.AcquireTokenAsync(shop, code);
            }

            _credentialStore.Set(HttpContext, shop, token);

            await _shopify.RegisterScriptTagAsync(shop, token);

            return RedirectToPage("/Greeting/HelloWorld", new { host, shop });
        }
    }
}
 
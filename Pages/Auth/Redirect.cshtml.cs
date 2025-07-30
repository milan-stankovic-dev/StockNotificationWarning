using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StockNotificationWarning.Services.Abstraction;

namespace StockNotificationWarning.Pages.Auth
{
    public class RedirectModel(IMetadataProvider metadataProvider,
                                IShopifyRequestService shopify) : PageModel
    {
        readonly IMetadataProvider _metadataProvider = metadataProvider;
        readonly IShopifyRequestService _shopify = shopify;
        public async Task<IActionResult> OnGetAsync(string code, string shop, string host)
        {
            string? token = (await _metadataProvider.Provide())["accessToken"];

            if (string.IsNullOrEmpty(token) || "N/A".Equals(token))
            {
                token = await _shopify.AcquireTokenAsync(shop, code);
            }

            await _shopify.RegisterScriptTagAsync(shop, token);

            return RedirectToPage("/Greeting/HelloWorld", new { host, shop });
        }
    }
}

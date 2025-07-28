using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StockNotificationWarning.Config;
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
            ShopifySessionStore.Host = host;

            if (string.IsNullOrEmpty(token) || "N/A".Equals(token))
            {
                token = await _shopify.AcquireTokenAsync(shop, code);
            }

            SaveShopifyStoreAsync(shop, token);

            await _shopify.RegisterScriptTagAsync(shop, token);

            //return RedirectToPage(
            //    $"https://admin.shopify.com/store/{shop}/apps/stocknotificationwarning?host={host}");
            return RedirectToPage("/Greeting/HelloWorld");
        }

        private void SaveShopifyStoreAsync(string shop, string token)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Expires = DateTimeOffset.UtcNow.AddDays(7)
            };

            ShopifySessionStore.ShopName = shop;
            ShopifySessionStore.AccessToken = token;
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Hosting;
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
            //ShopifySessionStore.Host = host;

            if (string.IsNullOrEmpty(token) || "N/A".Equals(token))
            {
                token = await _shopify.AcquireTokenAsync(shop, code);
            }

            await _shopify.RegisterScriptTagAsync(shop, token);

            return RedirectToPage("/Greeting/HelloWorld", new { host, shop });
        }

        //private void SaveShopifyStoreAsync(string shop, string token,string host)
        //{
        //    var cookieOptions = new CookieOptions
        //    {
        //        HttpOnly = true,
        //        Secure = true,
        //        SameSite = SameSiteMode.Lax,
        //        Expires = DateTimeOffset.UtcNow.AddDays(7)
        //    };

        //    Response.Cookies.Append("Shopify.Shop", shop, cookieOptions);
        //    Response.Cookies.Append("Shopify.AccessToken", token, cookieOptions);
        //    Response.Cookies.Append("Shopify.Host", host, cookieOptions);

        //    ShopifySessionStore.ShopName = shop;
        //    ShopifySessionStore.AccessToken = token;
        //}
    }
}

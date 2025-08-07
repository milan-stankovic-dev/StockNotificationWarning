using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StockNotificationWarning.Services.Abstraction;

namespace StockNotificationWarning.Pages.Auth
{
    public class RedirectModel(IShopifyRequestService shopify,
                               IShopifyCredentialStore credentialStore,
                               IMetafieldExtensionService metafieldService,
                               IMetaobjectExtensionService metaobjectService,
                               ILogger<RedirectModel> logger) : PageModel
    {
        readonly ILogger<RedirectModel> _logger = logger;
        readonly IShopifyRequestService _shopify = shopify;
        readonly IShopifyCredentialStore _credentialStore = credentialStore;
        readonly IMetafieldExtensionService _metafieldService = metafieldService;
        readonly IMetaobjectExtensionService _metaobjectService = metaobjectService;
        public async Task<IActionResult> OnGetAsync(string code, string shop, string host)
        {
            string? token = _credentialStore.Get(shop);

            if (string.IsNullOrEmpty(token) || "N/A".Equals(token))
            {
                token = await _shopify.AcquireTokenAsync(shop, code);
            }

            _credentialStore.Set(HttpContext, shop, token);

            await _shopify.RegisterScriptTagAsync(shop, token);

            await _metafieldService.EnsureMetafieldExistsAsync(
                shopDomain: shop,
                accessToken: token,
                namespaceVal: "custom",
                key: "shipping_weight",
                type: "number_decimal",
                ownerType: "PRODUCT",
                name: "Shipping Weight",
                description: "Custom shipping weight example property",
                storeFrontVisibility: true
            );

            //var scopes = _scopeService.GetAllScopes();

            // ^ nije neophodno, otkomentarisi ako treba da vidis da li su uspesno registrovani
            // novi scope-ovi kada se radi restart aplikacije.

            var vendorDefintionId = await _metaobjectService.EnsureVendorExistsAsync();
            _logger.LogInformation($"Vendor metaobject found with id: {vendorDefintionId}");

            return RedirectToPage("/Greeting/HelloWorld", new { host, shop });
        }
    }
}
 
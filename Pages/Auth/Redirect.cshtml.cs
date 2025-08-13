using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StockNotificationWarning.Services.Abstraction;
using System.Text.Json;

namespace StockNotificationWarning.Pages.Auth
{
    public class RedirectModel(IShopifyRequestService shopify,
                               IShopifyCredentialStore credentialStore,
                               IMetafieldExtensionService metafieldService,
                               IMetaobjectExtensionService metaobjectService,
                               ILogger<RedirectModel> logger,
                               ICustomProductService customProductService,
                               IShopifyScopeService scopeService) : PageModel
    {
        readonly ILogger<RedirectModel> _logger = logger;
        readonly IShopifyRequestService _shopify = shopify;
        readonly IShopifyCredentialStore _credentialStore = credentialStore;
        readonly IMetafieldExtensionService _metafieldService = metafieldService;
        readonly IMetaobjectExtensionService _metaobjectService = metaobjectService;
        readonly ICustomProductService _customProductService = customProductService;
        readonly IShopifyScopeService _scopeService = scopeService;
        public async Task<IActionResult> OnGetAsync(string code, string shop, string host)
        {
            string? token = _credentialStore.Get(shop);

            if (string.IsNullOrEmpty(token) || "N/A".Equals(token))
            {
                token = await _shopify.AcquireTokenAsync(shop, code);
            }

            _credentialStore.Set(HttpContext, shop, token);

            await _shopify.RegisterScriptTagAsync(shop, token);

            var scopes = await _scopeService.GetAllScopes();
            _logger.LogInformation($"Found scopes: {JsonSerializer.Serialize(scopes)}");

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

            // Povezi product i vendor.

            await _metafieldService.EnsureMetafieldExistsAsync(
                shopDomain: shop,
                accessToken: token,
                namespaceVal: "custom",
                key: "vendor_ref",
                type: "metaobject_reference",
                ownerType: "PRODUCT",
                name: "Vendor Reference",
                storeFrontVisibility: true,
                description: "This metafield represents ManyToOne relationship product ->vendor"
                ,
                metaobjectDefinitionId: vendorDefintionId.ToString()
            );

            _logger.LogInformation($"Vendor metaobject found with id: {vendorDefintionId}");

            // Povezi product sa vendorom

            await _customProductService.AssignVendorToProductAsync("gid://shopify/Product/8858179666170",
                "gid://shopify/Metaobject/151779377402");

            //return RedirectToPage("/Greeting/HelloWorld", new { host, shop });
            return RedirectToPage("/Products", new {area = "Admin", host, shop});
        }
    }
}
 
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StockNotificationWarning.Services.Abstraction;

namespace StockNotificationWarning.Pages.Api
{
    public class InventoryCheckModel(IShopifyRequestService shopify) : PageModel
    {
        readonly IShopifyRequestService _shopify = shopify;
        public async Task<JsonResult> OnGetAsync(string handle)
        {
            
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StockNotificationWarning.Dto;
using StockNotificationWarning.Services.Abstraction;

namespace StockNotificationWarning.Areas.Admin.Pages
{
    public class VendorsModel(IShopifyVendorService vendorService) : PageModel
    {
        readonly IShopifyVendorService _vendorService = vendorService;
        public IEnumerable<Vendor> Vendors { get; set; } = [];

        public async Task OnGetAsync()
        {
            Vendors = await _vendorService.GetAllAsync();
        }
    }
}

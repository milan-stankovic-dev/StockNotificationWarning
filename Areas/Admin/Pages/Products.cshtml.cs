using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShopifySharp;
using StockNotificationWarning.Dto;
using StockNotificationWarning.Services.Abstraction;

namespace StockNotificationWarning.Areas.Pages
{
    public class ProductsModel(IInventoryMonitorService inventoryMonitor) : PageModel
    {
        readonly IInventoryMonitorService _inventoryMonitor = inventoryMonitor;
        public List<Product> Products { get; set; } = [];
        public List<UnderstockedProductDto> UnderstockedProducts { get; set; } = [];
        public ProductsData CustomProducts { get; set; } = default!;

        [BindProperty(SupportsGet = true)]
        public bool Understocked { get; set; }

        public async Task OnGetAsync()
        {
            //if(Understocked)
            //{   
            //    UnderstockedProducts = [.. await _inventoryMonitor.FindUnderstockedProducts()];
            //    return;
            //}

            //Products = [.. await _inventoryMonitor.FindProducts()]; 

            // Privremeno zakomentarisan kod, testiram nesto za metafields

            CustomProducts = await _inventoryMonitor.FindProductsCustomFieldsAsync();
        }
    }
}

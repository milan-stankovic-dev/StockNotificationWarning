using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShopifySharp;
using StockNotificationWarning.Dto;
using StockNotificationWarning.Entities;
using StockNotificationWarning.Services.Abstraction;

namespace StockNotificationWarning.Areas.Pages
{
    public class ProductsModel(IInventoryMonitorService inventoryMonitor,
                                ICustomProductService customProductService,
                                IProductDetailsService productDetailsService) : PageModel
    {
        readonly IInventoryMonitorService _inventoryMonitor = inventoryMonitor;
        readonly ICustomProductService _customProductService = customProductService;
        readonly IProductDetailsService _productDetailsService = productDetailsService;
        public List<Product> Products { get; set; } = [];
        public List<UnderstockedProductDto> UnderstockedProducts { get; set; } = [];
        public List<ProductWithVendorDto> CustomProducts { get; set; } = default!;
        public List<ProductWithVendorAndDetailsDto> CustomProductWithDetails { get; set; } = [];

        [BindProperty(SupportsGet = true)]
        public bool Understocked { get; set; }

        public async Task OnGetAsync()
        {

            List<ProductWithVendorAndDetailsDto> enrichedList = [];

            //if(Understocked)
            //{   
            //    UnderstockedProducts = [.. await _inventoryMonitor.FindUnderstockedProducts()];
            //    return;
            //}

            //Products = [.. await _inventoryMonitor.FindProducts()]; 

            // Privremeno zakomentarisan kod, testiram nesto za metafields

            CustomProducts = await _customProductService.GetProductsWithVendorDtoAsync();

            foreach(var product in CustomProducts)
            {
                var productId = product.ProductId;
                IEnumerable<ProductDetails> detailsForProduct = 
                    await _productDetailsService.GetProductDetailsForAsync(productId!);

                enrichedList.Add(new()
                {
                    ProductId = productId,
                    ProductTitle = product.ProductTitle,
                    Vendor = product.Vendor,
                    Details = [.. detailsForProduct]
                });
            }

            CustomProductWithDetails = enrichedList;
        }

    }
}

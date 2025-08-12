using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StockNotificationWarning.Dto;
using StockNotificationWarning.Entities;
using StockNotificationWarning.Services.Abstraction;
using System.Threading.Tasks;

namespace StockNotificationWarning.Areas.Admin.Pages
{
    public class ProductDetailsModel(IProductDetailsService productDetailsService) : PageModel
    {
        readonly IProductDetailsService _problemDetailsService = productDetailsService;
        public string? ProductId { get; set; }
        public string? ProductName { get; set; }
        public List<ProductDetails> Details { get; set; } = [];

        public async Task<IActionResult> OnGet(string? productId, string? productName)
        {
            if(productId is null)
            {
                return NotFound();
            }

            ProductId = productId;
            ProductName = productName;
            Details = [.. await _problemDetailsService.GetProductDetailsForAsync(productId)];

            return Page();
        }
    }
}

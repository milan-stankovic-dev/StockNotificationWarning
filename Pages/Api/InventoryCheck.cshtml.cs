using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StockNotificationWarning.Services;
using StockNotificationWarning.Services.Abstraction;

namespace StockNotificationWarning.Pages.Api
{
    public class InventoryCheckModel(IInventoryMonitorService monitorService) : PageModel
    {
        readonly IInventoryMonitorService _monitorService = monitorService;
        public async Task<JsonResult> OnGetAsync()
        {
            var understockedProducts = 
                await _monitorService.FindUnderstockedProducts();

            return new JsonResult(understockedProducts);
        }
    }
}

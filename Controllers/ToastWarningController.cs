using Microsoft.AspNetCore.Mvc;
using StockNotificationWarning.Services.Abstraction;

namespace StockNotificationWarning.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ToastWarningController(IInventoryMonitorService inventoryMonitor,
                                        IToastNotificationService toastService) : ControllerBase
    {
        readonly IInventoryMonitorService _inventoryMonitor = inventoryMonitor;
        readonly IToastNotificationService _toastService = toastService;
        
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            await _inventoryMonitor.NotifyToastServiceOfUnderstocked();
            var toasts = _toastService.GetAllToasts();

            return Ok(toasts);
        }
    }
}

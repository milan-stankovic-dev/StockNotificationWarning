using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace StockNotificationWarning.Areas.Pages
{
    public class IndexModel : PageModel
    {
        public async Task<IActionResult> OnGetAsync()
        {
            var shop = Request.Query["shop"].ToString();
            return Redirect($"/Admin/Index?shop={shop}");
        }
    }
}

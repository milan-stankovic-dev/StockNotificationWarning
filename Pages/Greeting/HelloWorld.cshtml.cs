using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StockNotificationWarning.Config;

namespace StockNotificationWarning.Pages.Greeting
{
    public class HelloWorldModel : PageModel
    {
        public IActionResult OnGetAsync()
        {
            var host = Request.Query["host"].ToString();

            if(string.IsNullOrEmpty(host))
            {
                return Content("Could not find host");
                
            }
            ShopifySessionStore.Host = host;

            return Page();
        }
    }
}

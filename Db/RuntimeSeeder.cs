using Microsoft.EntityFrameworkCore;
using StockNotificationWarning.Entities;

namespace StockNotificationWarning.Db
{
    public static class RuntimeSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            await context.Database.MigrateAsync();

            if(!context.ProductDetails.Any())
            {
                context.ProductDetails.AddRange(
                new ProductDetails
                {
                    ShopifyProductGid = "gid://shopify/Product/8858179666170",
                    CustomDisplayName = "My Newest Product",
                    CustomDescription = "This is a detailed description for the newest product."
                },
                new ProductDetails
                {
                    ShopifyProductGid = "gid://shopify/Product/8858181271802",
                    CustomDisplayName = "Patike",
                    CustomDescription = "Comfortable sneakers for everyday use."
                },
                new ProductDetails
                {
                    ShopifyProductGid = "gid://shopify/Product/8861054828794",
                    CustomDisplayName = "Cizme Model X",
                    CustomDescription = "Premium boots for winter season."
                },
                new ProductDetails
                { 
                    ShopifyProductGid = "gid://shopify/Product/8861090283770",
                    CustomDisplayName = "My New Low Stock Product",
                    CustomDescription = "Keep an eye on inventory for this one!"
                },
                new ProductDetails
                {
                    ShopifyProductGid = "gid://shopify/Product/8861082714362",
                    CustomDisplayName = "Soko",
                    CustomDescription = "A classic design with modern comfort."
                }
            );

                await context.SaveChangesAsync();
            }
        }
    }
}

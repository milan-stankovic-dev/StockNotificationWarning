using Microsoft.EntityFrameworkCore;
using StockNotificationWarning.Db;
using StockNotificationWarning.Dto;
using StockNotificationWarning.Entities;
using StockNotificationWarning.Services.Abstraction;

namespace StockNotificationWarning.Services
{
    public class ProductDetailsService(ApplicationDbContext context) : IProductDetailsService
    {
        readonly ApplicationDbContext _context = context;

        public async Task<IEnumerable<ProductDetails>> GetProductDetailsForAsync(string productId) =>
            await _context.ProductDetails
                .Where(pd => (pd.ShopifyProductGid.Equals(productId))).ToListAsync();
    }
}

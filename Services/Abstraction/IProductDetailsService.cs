using StockNotificationWarning.Dto;
using StockNotificationWarning.Entities;

namespace StockNotificationWarning.Services.Abstraction
{
    public interface IProductDetailsService
    {
        Task<IEnumerable<ProductDetails>> GetProductDetailsForAsync(string productId);
    }
}

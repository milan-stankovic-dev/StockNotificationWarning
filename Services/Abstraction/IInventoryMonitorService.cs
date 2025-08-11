using ShopifySharp;
using StockNotificationWarning.Dto;

namespace StockNotificationWarning.Services.Abstraction
{
    public interface IInventoryMonitorService
    {
        Task<IEnumerable<UnderstockedProductDto>> FindUnderstockedProducts();
        Task<IEnumerable<Product>> FindProducts();
        Task NotifyToastServiceOfUnderstocked();
        Task<ProductsData> FindProductsCustomFieldsAsync();
    }
}

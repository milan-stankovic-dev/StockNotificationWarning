using StockNotificationWarning.Dto;

namespace StockNotificationWarning.Services.Abstraction
{
    public interface IShopifyVendorService
    {
        Task<IEnumerable<Vendor>> GetAllAsync();
    }
}

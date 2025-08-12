using StockNotificationWarning.Dto;

namespace StockNotificationWarning.Services.Abstraction
{
    public interface ICustomProductService
    {
        Task AssignVendorToProductAsync(string productId, string vendorId);
        Task<ProductsData> GetProductsWithVendorAsync();
        Task<List<ProductWithVendorDto>> GetProductsWithVendorDtoAsync();
    }
}

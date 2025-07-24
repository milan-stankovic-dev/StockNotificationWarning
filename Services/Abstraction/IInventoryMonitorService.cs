using StockNotificationWarning.Dto;

namespace StockNotificationWarning.Services.Abstraction
{
    public interface IInventoryMonitorService
    {
        Task<IEnumerable<UnderstockedProductDto>> FindUnderstockedProducts();
    }
}

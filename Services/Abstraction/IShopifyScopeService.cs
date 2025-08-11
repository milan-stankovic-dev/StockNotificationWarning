namespace StockNotificationWarning.Services.Abstraction
{
    public interface IShopifyScopeService
    {
        Task<string[]> GetAllScopes();
    }
}

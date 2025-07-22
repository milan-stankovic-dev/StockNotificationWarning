namespace StockNotificationWarning.Services.Abstraction
{
    public interface IMetadataProvider
    {
        Task<Dictionary<string, string>> Provide();
        Task<Dictionary<string, string>> Provide(string? defaultShopName);
    }
}

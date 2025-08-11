namespace StockNotificationWarning.Services.Abstraction
{
    public interface IMetaobjectExtensionService
    {
        Task CreateVendorAsync(long? vendorDefinitionId);
        Task<long> EnsureVendorExistsAsync();
    }
}

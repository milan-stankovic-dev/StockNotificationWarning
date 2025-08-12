namespace StockNotificationWarning.Services.Abstraction
{
    public interface IMetafieldExtensionService
    {
        Task AddMetafield(string? key, string? value,
            string? valueType, string? ownerResource, int? ownerId,
            string? shopDomain, string? accessToken);
        Task CreateMetafieldDefinitionAsync(
                                   string? shopDomain,
                                   string? accessToken,
                                   string? name,
                                   string? namespaceValue,
                                   string? key,
                                   string? type,
                                   string? ownerType,
                                   string? description = "",
                                   bool storefrontVisibility = false,
                                   string? metaobjectDefinitionId = null);
        Task EnsureMetafieldExistsAsync(string shopDomain,
                                        string accessToken, 
                                        string namespaceVal, 
                                        string key, 
                                        string type, 
                                        string ownerType, 
                                        string name, 
                                        string description, 
                                        bool storeFrontVisibility,
                                        string? metaobjectDefinitionId = null);
    }
}

using ShopifySharp;
using ShopifySharp.Filters;
using ShopifySharp.Lists;
using StockNotificationWarning.Dto;
using StockNotificationWarning.Services.Abstraction;

namespace StockNotificationWarning.Services
{
    public class InventoryService(IMetadataProvider metadataProvider,
                                  ILogger<InventoryService> logger,
                                  IToastNotificationService toastService)
    {
        readonly IMetadataProvider _metadataProvider = metadataProvider;
        readonly ILogger<InventoryService> _logger = logger;
        readonly IToastNotificationService _toastService = toastService;
        public async Task<IEnumerable<UnderstockedProductDto>> FetchUnderstockedProducts()
        {
            string? shop = (await _metadataProvider.Provide()).GetValueOrDefault("shopName");
            string? token = (await _metadataProvider.Provide()).GetValueOrDefault("accessToken");

            if ("N/A".Equals(shop) || "N/A".Equals(token))
            {
                //Necemo ovde da bacamo jer onda na samom pocetku puca aplikacija,
                //eagerly pokusava da izvrsi servisnu metodu, a nije jos registrovan token u sistemu
                //cekamo...

                _logger.LogWarning("Shop or token did not get initialized. Waiting before retry...");
                return [];
            }

            var inventoryService = new InventoryLevelService(shop, token);

            var locationIds = await GetLocationIds(shop, token);

            if (locationIds is null || locationIds.Count == 0)
            {
                _logger.LogWarning("No location IDs found. Cannot fetch inv levels");
                return [];
            }

            var filter = new InventoryLevelListFilter
            {
                LocationIds = locationIds,
                Limit = 250
            };

            var inventoryLevels = await inventoryService.ListAsync(filter);
            _toastService.ClearWarnings();
            await SetUpWarningMessages(inventoryLevels);
        }

        async Task<List<long>> GetLocationIds(string shop, string token)
        {
            var locationService = new LocationService(shop, token);
            var locations = await locationService.ListAsync();
            return [.. locations.Items.Select(loc => loc!.Id!.Value)];
        }

        async Task SetUpWarningMessages(ListResult<InventoryLevel> levels)
        {
            foreach (var level in levels.Items)
            {
                if (level.Available < 10)
                {

                    string? title = await GetProductTitleFromInvItemIdGraphQL(level.InventoryItemId!.Value);

                    if (title is null)
                    {
                        continue;
                    }

                    string productName = title;

                    string warningMessage = $"Low stock found for item: {productName}. Stock:" +
                        $"{level.Available}";
                    _logger.LogInformation($"PRODUCT FOUND WITH LOWER STOCK: {productName} = {level.Available}");
                    _notificationService.AddToast(warningMessage);
                }
            }
        }
    }
}

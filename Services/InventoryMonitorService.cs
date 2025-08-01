using ShopifySharp;
using ShopifySharp.Filters;
using ShopifySharp.Lists;
using StockNotificationWarning.Dto;
using StockNotificationWarning.Services.Abstraction;

namespace StockNotificationWarning.Services
{
    public class InventoryMonitorService(
        IServiceProvider services,
        ILogger<InventoryMonitorService> logger,
        IServiceProvider serviceProvider,
        IToastNotificationService notificationService,
        IShopifyContextService contextService) : IInventoryMonitorService
    {
        readonly IToastNotificationService _notificationService = notificationService;
        readonly IServiceProvider _services = services;
        readonly ILogger<InventoryMonitorService> _logger = logger;
        readonly IServiceProvider _serviceProvider = serviceProvider;
        readonly IShopifyContextService _contextService = contextService;

        async Task<string?> GetProductTitleFromInvItemIdGraphQL(long invItemId)
        {
            using var scope = _services.CreateScope();

            var metadataProvider = scope.ServiceProvider.GetRequiredService<IShopifyContextService>();
            var shop = metadataProvider.Shop;
            var token = metadataProvider.AccessToken;

            if(shop is null || token is null || "N/A".Equals(shop) ||
                "N/A".Equals(token))
            {
                _logger.LogWarning($"Could not call upon the graph service due to shop being" +
                    $" set to {shop} and token is {token}. That combination of values is invalid.");
                
                return null;
            }

            var gql = new GraphService(shop, token);

            var query = @"
            query getTitle($invId: ID!) {
                inventoryItem(id: $invId) {
                    variant {
                        product {
                            title
                        }
                    }
                }
            }
            ";

            string gid = $"gid://shopify/InventoryItem/{invItemId}";

            var response = await gql.PostAsync<GQLResponse>(
            new GraphRequest
            {
                Query = query,
                Variables = new Dictionary<string, object?>
                {
                    ["invId"] = gid
                }!
            });

            return response.Data.InventoryItem?.Variant?.Product?.Title;
        }

        class GQLResponse
        {
            public InventoryItemResponse? InventoryItem { get; set; }
        }

        class InventoryItemResponse
        {
            public VariantResponse? Variant { get; set; }
        }

        class VariantResponse
        {
            public ProductResponse? Product { get; set; }
        }

        class ProductResponse
        {
            public string? Title { get; set; }
        }

        async Task<IEnumerable<UnderstockedProductDto>> FormDtoCollection(ListResult<InventoryLevel> levels)
        {
            var levelsCount = levels.Items.Count();
            var result = new List<UnderstockedProductDto>(levelsCount);

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

                    //string warningMessage = $"Low stock found for item: {productName}. Stock:" +
                    //$"{level.Available}";
                    //_logger.LogInformation($"PRODUCT FOUND WITH LOWER STOCK: {productName} = {level.Available}");
                    //_notificationService.AddToast(warningMessage);

                    var dto = new UnderstockedProductDto
                    {
                        ProductName = productName,
                        Stock = level.Available
                    };

                    result.Add(dto);
                }
            }

            return [.. result];
        }

        async Task<List<long>> GetLocationIds(string shop, string token)
        {
            var locationService = new LocationService(shop, token);
            var locations = await locationService.ListAsync();
            return [.. locations.Items.Select(loc => loc!.Id!.Value)];
        }

        public async Task<IEnumerable<UnderstockedProductDto>> FindUnderstockedProducts()
        {
            using var scope = _serviceProvider.CreateScope();
            var metadataProvider = scope.ServiceProvider
                                        .GetRequiredService<IShopifyContextService>();

            //string? shop = (await metadataProvider.Provide()).GetValueOrDefault("shopName");
            //string? token = (await metadataProvider.Provide()).GetValueOrDefault("accessToken");

            var shop = metadataProvider.Shop;
            var token = metadataProvider.AccessToken;

            if ( shop is null || token is null || "N/A".Equals(shop) || "N/A".Equals(token) ||
                "".Equals(token) || "".Equals(shop))

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
            _notificationService.ClearWarnings();
            return await FormDtoCollection(inventoryLevels);
        }

        public async Task<IEnumerable<Product>> FindProducts()
        {
            using var scope = _serviceProvider.CreateScope();

            //string? shop = (await metadataProvider.Provide()).GetValueOrDefault("shopName");
            //string? token = (await metadataProvider.Provide()).GetValueOrDefault("accessToken");

            var shop = _contextService.Shop;
            var token = _contextService.AccessToken;

            var productService = new ProductService(shop, token);

            var products = await productService.ListAsync();

            return products.Items;
        }

        public async Task NotifyToastServiceOfUnderstocked()
        {
            var understockedProds = await FindUnderstockedProducts();

            foreach(var prod in understockedProds)
            {
                _notificationService.AddToast("Found low stock for product " + prod.ProductName + ". Stock: " + prod.Stock);
            }
        }
    }
}

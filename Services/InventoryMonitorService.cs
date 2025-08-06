using ShopifySharp;
using ShopifySharp.Filters;
using ShopifySharp.Lists;
using StockNotificationWarning.Dto;
using StockNotificationWarning.Services.Abstraction;

namespace StockNotificationWarning.Services
{
    public class InventoryMonitorService(
        ILogger<InventoryMonitorService> logger,
        IServiceProvider serviceProvider,
        IToastNotificationService notificationService) : IInventoryMonitorService
    {
        readonly IToastNotificationService _notificationService = notificationService;

        readonly ILogger<InventoryMonitorService> _logger = logger;
        readonly IServiceProvider _serviceProvider = serviceProvider;
        public async Task<ProductsData> FindProductsCustomFieldsAsync()
        {
            var (shop, token) = GetShopAndToken();

            var gql = new GraphService(shop, token);

            var query = @"
            query GetProductsWithMetafields($first: Int!, $after: String) {
                products(first: $first, after: $after) {
                    edges {
                        cursor
                        node {
                            id
                            title
                            metafield(namespace: ""custom"", key: ""shipping_weight"") {
                                id
                                namespace
                                key
                                value
                                type 
                            }
                        }
                    }
                }
            }
            ";

            var variables = new Dictionary<string, object>
            {
                { "first", 10 }
            };

            var response = await gql.PostAsync<ProductsData>(
                    new GraphRequest
                    {
                        Query = query,
                        Variables = variables
                    }
            );
            _logger.LogInformation(response.Data.ToString());

            return response.Data;
        }

        async Task<string?> GetProductTitleFromInvItemIdGraphQL(long invItemId)
        {
            var (shop, token) = GetShopAndToken();

            if (shop is null || token is null || "N/A".Equals(shop) ||
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

            var response = await gql.PostAsync<GQLDefaultResponse>(
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

        public class GQLDefaultResponse
        {
            public InventoryItemDefaultResponse? InventoryItem { get; set; }
        }

        public class InventoryItemDefaultResponse
        {   
            public VariantDefaultResponse? Variant { get; set; }
        }

        public class VariantDefaultResponse
        {
            public ProductDefaultResponse? Product { get; set; }
        }
        
        public class ProductCustomWeightFieldResponse
        {
            public string? Title { get; set; }
            public decimal? Weight { get; set; }
        }

        public class ProductDefaultResponse
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
            var (shop, token) = GetShopAndToken();

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

        (string shop, string token) GetShopAndToken()
        {
            using var scope = _serviceProvider.CreateScope();
            var configDefaultsProvider = scope.ServiceProvider.GetRequiredService<IConfigDefaultsProvider>();
            var credentialStore = scope.ServiceProvider.GetRequiredService<IShopifyCredentialStore>();

            var shop = configDefaultsProvider.Provide();
            var token = credentialStore.Get(shop) ?? "N/A";

            return (shop, token);
        }

        public async Task<IEnumerable<Product>> FindProducts()
        {
            var (shop, token) = GetShopAndToken();

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

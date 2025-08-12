using ShopifySharp;
using StockNotificationWarning.Dto;
using StockNotificationWarning.Services.Abstraction;
using System.Text.Json;

namespace StockNotificationWarning.Services
{
    public class CustomProductService(IShopTokenProvider shopTokenProvider,
                                        ILogger<CustomProductService> logger) : ICustomProductService
    {
        readonly IShopTokenProvider _shopTokenProvider = shopTokenProvider;
        readonly ILogger<CustomProductService> _logger = logger;
        public async Task AssignVendorToProductAsync(string productId, string vendorId)
        {
            (string shop, string token) = _shopTokenProvider.Provide();
            var gql = new GraphService(shop, token);

            var mutation = @"
            mutation AssignVendorToProduct($productId: ID!, $vendorId: String!) {
                metafieldsSet(
                    metafields: [ 
                    {
                        ownerId: $productId
                        namespace: ""custom""
                        key: ""vendor_ref""
                        type: ""metaobject_reference""
                        value: $vendorId
                    }
                  ]
                ) {
                     metafields {
                        id
                        value
                     }
                     userErrors {
                        field
                        message
                     }
                  }
                }";

            var variables = new Dictionary<string, object?>
            {
                ["productId"] = productId,
                ["vendorId"] = vendorId
            };

            var result = await gql.PostAsync(new GraphRequest
            {
                Variables = variables!,
                Query = mutation
            });

            _logger.LogInformation($"Product vendor assign operation response received: {result.Json.GetRawText()}");
        }

        public async Task<ProductsData> GetProductsWithVendorAsync()
        {
            (string shop, string token) = _shopTokenProvider.Provide();
            var gql = new GraphService(shop, token);

            var query = @"
                query GetProductsWithVendor($first: Int!) {
                  products(first: $first) {
                    edges {
                      cursor
                      node {
                        id
                        title
                        metafield(namespace: ""custom"", key: ""vendor_ref"") {
                          reference {
                            ... on Metaobject {
                              id
                              handle
                              fields {
                                key
                                value
                              }
                            }
                          }
                        }
                      }
                    }
                  }
                }";

            var variables = new Dictionary<string, object?>
            {
                ["first"] = 250
            };

            var response = await gql.PostAsync<ProductsData>(new GraphRequest
            {
                Query = query,
                Variables = variables!
            });

            _logger.LogInformation($"Fetched products with vendor successfully! {JsonSerializer.Serialize(response.Data)}");

            return response.Data!;
        }

        public List<ProductWithVendorDto> FormProperDtos(ProductsData productsData)
        {
            var result = new List<ProductWithVendorDto>();

            if (productsData?.Products?.Edges == null)
                return result;

            foreach (var edge in productsData.Products.Edges)
            {
                var product = edge.Node;
                if (product == null)
                    continue;

                Vendor? vendor = null;

                var metaRef = product.Metafield?.Reference;
                if (metaRef != null)
                {
                    vendor = new Vendor
                    {
                        Id = ParseIdFromGid(metaRef.Id),
                        Name = metaRef.Fields.FirstOrDefault(f => f.Key == "name")?.Value,
                        Description = metaRef.Fields.FirstOrDefault(f => f.Key == "description")?.Value,
                        WebsiteUrl = metaRef.Fields.FirstOrDefault(f => f.Key == "website_url")?.Value
                    };
                }

                result.Add(new ProductWithVendorDto
                {
                    ProductId = product.Id,
                    ProductTitle = product.Title,
                    Vendor = vendor
                });
            }

            return result;
        }

        private long? ParseIdFromGid(string? gid)
        {
            if (string.IsNullOrWhiteSpace(gid))
                return null;

            var parts = gid.Split('/');
            if (long.TryParse(parts.LastOrDefault(), out var id))
                return id;

            return null;
        }

        public async Task<List<ProductWithVendorDto>> GetProductsWithVendorDtoAsync()
        {
            var apiResponse = await GetProductsWithVendorAsync();
            return FormProperDtos(apiResponse);
        }
    }
}

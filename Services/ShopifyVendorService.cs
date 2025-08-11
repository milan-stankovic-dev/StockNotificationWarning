using ShopifySharp;
using StockNotificationWarning.Dto;
using StockNotificationWarning.Services.Abstraction;
using System.Text.Json;

namespace StockNotificationWarning.Services
{
    public class ShopifyVendorService(IShopTokenProvider shopTokenProvider,
                                        ILogger<ShopifyVendorService> logger) : IShopifyVendorService
    {
        readonly IShopTokenProvider _shopTokenProvider = shopTokenProvider;
        readonly ILogger<ShopifyVendorService> _logger = logger;
        public async Task<IEnumerable<Vendor>> GetAllAsync()
        {
            //VendorMetaobjectResponse
            (string shop, string token) = _shopTokenProvider.Provide();

            var gql = new GraphService(shop, token);
            string query = @"
            query GetVendors($first: Int!) {
                metaobjects(first: $first, type: ""vendor"") {
                    edges {
                        node {
                            id
                            handle
                            fields {
                                key
                                value
                            }
                        }
                    }
                }
            }";

            var variables = new Dictionary<string, object?>
            {
                ["first"] = 100
            };
                var response = await gql.PostAsync<VendorMetaobjectListResponse>(new GraphRequest
                {
                    Query = query,
                    Variables = variables!
                });

            var responseData = response.Data;

            _logger.LogInformation($"Successfully fetched vendor data! {JsonSerializer.Serialize(responseData)}");

            return VendorListResponseToListOfVendorDtos(responseData);
        }
        List<Vendor> VendorListResponseToListOfVendorDtos(VendorMetaobjectListResponse listResponse)
        {
            if (listResponse?.Metaobjects?.Edges == null)
                return new List<Vendor>();

            var vendors = listResponse.Metaobjects.Edges.Select(edge =>
            {
                var node = edge.Node;
         
                long? numericId = null;
                if (!string.IsNullOrEmpty(node.Id))
                {
                    var parts = node.Id.Split('/');
                    if (long.TryParse(parts.Last(), out var id))
                        numericId = id;
                }

                var fieldsDict = node.Fields?
                    .ToDictionary(f => f.Key.ToLowerInvariant(), f => f.Value)
                    ?? new Dictionary<string, string>();

                return new Vendor
                {
                    Id = numericId,
                    Name = fieldsDict.TryGetValue("name", out var name) ? name : null,
                    Description = fieldsDict.TryGetValue("description", out var desc) ? desc : null,
                    WebsiteUrl = fieldsDict.TryGetValue("website_url", out var url) ? url : null
                };
            }).ToList();

            return vendors;
        }
    }

        
}

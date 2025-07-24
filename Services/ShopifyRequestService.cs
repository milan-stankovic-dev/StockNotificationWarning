using Microsoft.Extensions.Options;
using ShopifySharp;
using StockNotificationWarning.Config;
using StockNotificationWarning.Services.Abstraction;
using System.Text.Json;

namespace StockNotificationWarning.Services
{
    public class ShopifyRequestService(IOptionsMonitor<ShopifyConfig> options) : IShopifyRequestService
    {
        private readonly ShopifyConfig _config = options.CurrentValue;
        readonly JsonSerializerOptions jsonOptions = new() { PropertyNameCaseInsensitive = true };
        
        public async Task ActivateAsync(long productId, string shop, string accessToken)
        {
            var gql = new GraphService(shop, accessToken);

            var query = @"
                  mutation productUpdate($input: ProductInput!) {
                    productUpdate(input: $input) {
                        product {
                            id
                            status
                      }
                    }
                    userErrors {
                        field
                        message
                    }
                  }
            ";

            var variables = new Dictionary<string, object>
            {
                { "id", $"gid://shopify/Product/{productId}" },
                { "status", "active" }
            };

            var gqlQuery = new GraphRequest
            {
                Query = query,
                Variables = variables
            };

            var response = await gql.PostAsync(gqlQuery);
        }

        public async Task DeleteAsync(long productId, string shop, string accessToken)
        {
            var gql = new GraphService(shop, accessToken);

            var query = @"
                mutation productDelete($input: ProductDeleteInput!) {
                    productDelete(input: $input) {
                        deletedProductId
                        userErrors {
                            field
                            message
                        }
                    }
                }
            ";

            var variables = new Dictionary<string, object>
            {
                { "id", $"gid://shopify/Product/{productId}" }
            };

            var gqlQuery = new GraphRequest
            {
                Query = query,
                Variables = variables
            };

            var response = await gql.PostAsync(gqlQuery);
        }

        public async Task DeleteAllWithStatus(string status, string shop, string accessToken)
        {
            var gql = new GraphService(shop, accessToken);

            var query = @"
            {
              products(first: 100) {
                 edges {
                   node {
                      id
                      title
                      status
                  }
                }
              }
            }";

            var gqlQuery = new GraphRequest
            {
                Query = query
            };

            var response = await gql.PostAsync(gqlQuery);
            var jsonString = response.Json.GetRawText();

            var typedResponse = JsonSerializer.Deserialize<DeleteGraphResponse>(
                jsonString, 
                jsonOptions
            );

            foreach(var edge in typedResponse!.Products.Edges)
            {
                var id = edge.Node.Id;
                var numericId = long.Parse(id.Split('/').Last());

                await DeleteAsync(numericId, shop, accessToken);
            }
        }

        public class DeleteGraphResponse
        {
            public ProductsContainer Products { get; set; } = new();
        }

        public class ProductsContainer
        {
            public List<DeleteGraphEdge> Edges { get; set; } = new();
        }

        public class DeleteGraphEdge
        {
            public DeleteGraphNode Node { get; set; } = new();
        }

        public class DeleteGraphNode
        {
            public string Id { get; set; } = string.Empty;
            public string? Title { get; set; }
            public string? Status { get; set; }
        }

        public async Task ActivateAllAsync(string shop, string accessToken)
        {
            var gqlService = new GraphService(shop, accessToken);

            var query = @"
            {
                products(first: 100, query: ""status:"" draft) {
                    edges {
                        node {
                            id
                            title
                            status
                        }
                    }
                }
            }";

            var gqlQuery = new GraphRequest
            {
                 Query = query
            };

            var response = await gqlService.PostAsync(gqlQuery);
            string responseJson = response.Json.GetRawText();

            var typedResponse = JsonSerializer.Deserialize<DeleteGraphResponse>
                (responseJson, jsonOptions);

            foreach (var edge in typedResponse!.Products.Edges)
            {
                var gid = edge.Node.Id;
                var numericId = long.Parse(gid.Split('/').Last());

                if (edge is null)
                {
                    continue;
                }

                await ActivateAsync(numericId, shop, accessToken);
            }
        }

        public string BuildAuthorizationUrl(string shop)
        {
            var uri = AuthorizationService.BuildAuthorizationUrl(
                _config.Scopes.Split(','),
                shop,
                _config.ApiKey,
                _config.RedirectUri
                );

            return uri.ToString();
        }

        public async Task<string> AcquireTokenAsync(string shop, string code)
            => await AuthorizationService.Authorize(
                code, shop, _config.ApiKey, _config.SecretKey);

        public async Task RegisterScriptTagAsync(string shop, string token)
        {
            var service = new ScriptTagService(shop, token);

            var scriptTag = new ScriptTag
            {
                Event = "onload",
                Src = "example"
            };

            var existingTags = await service.ListAsync();
            bool alreadyRegistered = existingTags.Items.Any(tag => tag.Src.Equals(scriptTag.Src));

            if (!alreadyRegistered)
            {
                await service.CreateAsync(scriptTag);
            }
        }
    }
}

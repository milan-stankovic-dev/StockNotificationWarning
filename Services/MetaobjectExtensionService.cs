using ShopifySharp;
using StockNotificationWarning.Dto;
using StockNotificationWarning.Services.Abstraction;

namespace StockNotificationWarning.Services
{
    public class MetaobjectExtensionService(IShopTokenProvider shopTokenProvider,
                                            ILogger<MetaobjectExtensionService> logger) : IMetaobjectExtensionService
    {
        readonly IShopTokenProvider _shopTokenProvider = shopTokenProvider;
        readonly ILogger<MetaobjectExtensionService> _logger = logger;

        public async Task MetaobjectDefinitionCreateAsync()
        {
            (string shop, string token) = _shopTokenProvider.Provide();
            var gql = new GraphService(shop, token);
            var mutation = @"
            mutation {
                metaobjectDefinitionCreate(definition: {
                    name: ""Vendor"",
                    type: ""vendor"",
                    fieldDefinitions: [
                        { key: ""name"", name: ""Name"", type: ""single_line_text_field"" },
                        { key: ""description"", name: ""Description"", type: ""multi_line_text_field"" },
                        { key: ""website_url"", name: ""Website URL"", type: ""url"" }
                    ]
                }) {
                    metaobjectDefinition {
                        id
                        name
                        type
                    }
                    userErrors {
                        field
                        message
                    }
                }
            }";

            var response = await gql.PostAsync<CreateMetaobjectDefinitionResponse>(
                new GraphRequest { Query = mutation }
            );

            if (response.Data.UserErrors.Any())
            {
                var errors = string.Join(", ", response.Data
                    .UserErrors.Select(e => e.Message));

                throw new Exception($"Failed to create Vendor metaobject definition. Error: {errors}");
            }

            _logger.LogInformation($"Vendor Metaobject definition with id: {response.Data.MetaobjectDefinition.Id} ");
        }

        public async Task CreateVendorAsync(long? vendorDefinitionId)
        {
            (string shop, string token) = _shopTokenProvider.Provide();
            var gql = new GraphService(shop, token);
            var query = @"
            mutation CreateVendor($definitionId: ID!, $fields: [MetaobjectFieldInput!]!) { 
                metaobjectCreate(input: {
                    definitionId: $definitionId,
                    fields: $fields
                }) {
                    metaobject {
                        id
                        handle
                        fields {
                            key
                            value
                        }
                    }
                    userErrors {
                        field
                        message
                    }
                }
            }";

            var definitionId = $"gid://shopify/MetaobjectDefinition/{vendorDefinitionId}";

            var variables = new Dictionary<string, object?>
            {
                ["definitionId"] = definitionId,
                ["fields"] = new[]
                {
                    new Dictionary<string, object?> {
                        ["key"] = "name",
                        ["value"] = "Acme Corp"
                    },
                    new Dictionary<string, object?> {
                        ["key"] = "description",
                        ["value"] = "A leading vendor of premium products."
                    },
                    new Dictionary<string, object?> {
                        ["key"] = "website_url",
                        ["value"] = "https://acme.example.com/"
                    },
                }
            };

            var response = await gql.PostAsync<VendorMetaobjectResponse>(
                new GraphRequest
                {
                    Query = query,
                    Variables = variables!
                }
            );

            if (response.Data.MetaobjectCreate.UserErrors.Any())
            {
                var errors = string.Join(", ", response.Data
                    .MetaobjectCreate.UserErrors.Select(e => e.Message));
                throw new Exception($"Failed to create vendor metaobject: ");
            }

            var createdVendor = response.Data.MetaobjectCreate.Metaobject;
            _logger.LogInformation($"Created vendor metaobject with ID: {createdVendor.Id}");
        }



        public async Task<long> EnsureVendorExistsAsync()
        {
            (string shop, string token) = _shopTokenProvider.Provide();
            var gql = new GraphService(shop, token);

            var definitionQuery = @"
            query {
                metaobjectDefinitions(first: 10) {
                    nodes {
                        id
                        name
                        type
                    }
                }
            }
            ";

            var definitionResponse = await gql.PostAsync<MetaobjectDefinitionQueryResponse>(
                new GraphRequest { Query = definitionQuery }
            );

            var vendorDefinition = definitionResponse.Data
                .MetaobjectDefinitions.Nodes.FirstOrDefault(x => "vendor".Equals(x.Type));

            if (vendorDefinition is not null)
            {
                _logger.LogInformation("Vendor MetaobjectDefinition already exists.");
                var parts = vendorDefinition.Id.Split('/');
                return long.Parse(parts.Last());
            }

            await MetaobjectDefinitionCreateAsync();

            var newDefinitionResponse = await gql.PostAsync<MetaobjectDefinitionQueryResponse>(
        new GraphRequest { Query = definitionQuery }
                );

            var createdDefinition = newDefinitionResponse.Data
                .MetaobjectDefinitions
                .Nodes
                .FirstOrDefault();

            if (createdDefinition == null)
                throw new Exception("Failed to create vendor MetaobjectDefinition.");

            _logger.LogInformation("Created Vendor MetaobjectDefinition.");
            var idParts = createdDefinition.Id.Split('/');
            return long.Parse(idParts.Last());
        }
    }
}

using ShopifySharp;
using StockNotificationWarning.Dto;
using StockNotificationWarning.Services.Abstraction;
using System.Text;
using System.Text.Json;

namespace StockNotificationWarning.Services
{

    public class MetafieldExtensionService(ILogger<MetafieldExtensionService> logger,
                                           IShopTokenProvider shopTokenProvider) : IMetafieldExtensionService
    {
        readonly ILogger<MetafieldExtensionService> _logger = logger;
        readonly IShopTokenProvider _shopTokenProvider = shopTokenProvider;
        public async Task AddMetafield(string? key, string? value,
            string? valueType, string? ownerResource, int? ownerId, string? shopDomain, string? accessToken)
        {
            var service = new MetaFieldService(shopDomain, accessToken);
            var metafield = await service.CreateAsync(new MetaField()
            {
                Namespace = "custom",
                Key = key,
                Value = value,
                OwnerResource = ownerResource,
                OwnerId = ownerId,
                CreatedAt = DateTime.UtcNow,
            });
        }

        string EscapeGraphQLString(string? input)
        {
            if (input is null) return "";

            return input.Replace("\\", "\\\\").Replace("\"", "\\\"");
        }

        public async Task CreateMetafieldDefinitionAsync(
                                    string? shopDomain,
                                    string? accessToken,
                                    string? name,
                                    string? namespaceValue,
                                    string? key,
                                    string? type,
                                    string? ownerType,
                                    string? description = "",
                                    bool storefrontVisibility = false,
                                    string? metaobjectDefinitionId = null)
        {
            var validationsSection = "";

            if ("metaobject_reference".Equals(type) && !string.IsNullOrEmpty(metaobjectDefinitionId))
            {
                validationsSection = $@"
                    validations: [
                    {{
                        name: ""metaobject_definition_id"",
                        value: ""gid://shopify/MetaobjectDefinition/{metaobjectDefinitionId}""                    
                    }}
                    ]";
            }

            var mutation = $@"
            mutation {{
                metafieldDefinitionCreate(definition: {{
                    name: ""{EscapeGraphQLString(name)}"",
                    namespace: ""{EscapeGraphQLString(namespaceValue)}"",
                    key: ""{EscapeGraphQLString(key)}"",
                    description: ""{EscapeGraphQLString(description)}"",
                    type: ""{EscapeGraphQLString(type)}"",
                    ownerType: {ownerType},
                    {validationsSection},
                    access: {{
                        admin: PUBLIC_READ_WRITE,
                        storefront: PUBLIC_READ
                    }}
                }}) {{
                    createdDefinition {{
                        name
                        namespace
                        key
                        type {{
                            name
                        }}
                        access {{
                            admin
                            storefront
                        }}
                    }}
                    userErrors {{
                        field
                        message
                        code
                    }}
                }}
            }}";

            var variables = new Dictionary<string, object?>
            {
                ["definition"] = new
                {
                    name = name,
                    @namespace = namespaceValue,
                    key = key,
                    type = type,
                    description = description,
                    ownerType = ownerType,
                    visibleToStorefront = storefrontVisibility

                }
            };

            (string shop, string token) = _shopTokenProvider.Provide();
            var gql = new GraphService(shop, token);

            var response = await gql.PostAsync(new GraphRequest
            {
                Query = mutation,
                Variables = variables!
            });

            var responseDto = JsonToTIgnoreCase
                <GQLGenericResponse<MetafieldDefinitionCreateResponse>>
                (response.Json.GetRawText());

            _logger.LogInformation($"*****IMPORTANT******. GQL RESPONSE: {JsonSerializer.Serialize(responseDto?.Data, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true
            })}");

            var errors = responseDto?.Data?.MetafieldDefinitionCreate?.UserErrors;

            string fullErrorMessage = "";

            if((errors?.Count ?? -1) > 0)
            {
                foreach(var error in errors!)
                {
                    fullErrorMessage += (error + "\n");
                }
            }

            if(!string.IsNullOrEmpty(fullErrorMessage))
            {
                throw new Exception($"Error(s) occurred during creation of metafield definition: {fullErrorMessage}");
            }

            _logger.LogInformation($"Metafield definition created {response.Json.GetRawText()}");
        }

        static readonly JsonSerializerOptions jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        T JsonToTIgnoreCase<T>(string input) =>
            JsonSerializer.Deserialize<T>(
                input, jsonOptions)!;
            
        public async Task EnsureMetafieldExistsAsync(string shopDomain, string accessToken,
                                             string namespaceVal, string key,
                                             string type, string ownerType,
                                             string name, string description,
                                             bool storeFrontVisibility,
                                             string? metaobjectDefinitionId = null)
        {
            var query = @"
                query metafieldDefinitions($first: Int!, $ownerType: MetafieldOwnerType!) {
                        metafieldDefinitions(first: $first, ownerType: $ownerType) {
                            edges {
                                node {
                                    id
                                    name
                                    namespace
                                    key
                                }
                            }
                        }
                 }";


            (string shop, string token) = _shopTokenProvider.Provide();

            var gql = new GraphService(shop, token);

            var variables = new Dictionary<string, object?>
            {
                ["first"] = 100,
                ["ownerType"] = ownerType!
            };

            var result = await gql.PostAsync(new GraphRequest
            {
                Query = query,
                Variables = variables!
            });

            var resultDto = JsonToTIgnoreCase
                <GQLGenericResponse<MetafieldDefinitionsResponse>>
                (result.Json.GetRawText());

            _logger.LogInformation($"Fetched all metafield definitions: {result.Json.GetRawText()}");

            var edges = resultDto?.Data?.MetafieldDefinitions?.Edges ?? [];
            _logger.LogInformation($"EDGES: {edges}");

            foreach(var edge in edges)
            {
                _logger.LogInformation($"EDGE: {edge}");

                if(edge?.Node?.Namespace == namespaceVal && edge?.Node.Key == key)
                {
                    _logger.LogInformation($"Metafield definition already exists: {namespaceVal}.{key}" +
                                $" (Name: {edge?.Node?.Name ?? ""})");
                    return;
                }
            }

            _logger.LogInformation($"Metafield definition not found, creating {namespaceVal}.{key}");

            await CreateMetafieldDefinitionAsync(shopDomain, accessToken, name,
                namespaceVal, key, type, ownerType, description, storeFrontVisibility, metaobjectDefinitionId);
        }
    }
}
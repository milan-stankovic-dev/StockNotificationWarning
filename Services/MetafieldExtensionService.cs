using ShopifySharp;
using StockNotificationWarning.Services.Abstraction;
using System.Text;
using System.Text.Json;

namespace StockNotificationWarning.Services
{

    public class MetafieldExtensionService(ILogger<MetafieldExtensionService> logger) : IMetafieldExtensionService
    {
        readonly ILogger<MetafieldExtensionService> _logger = logger;
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
                                    bool storefrontVisibility = false)
        {

            var mutation = $@"
            mutation {{
                metafieldDefinitionCreate(definition: {{
                    name: ""{EscapeGraphQLString(name)}"",
                    namespace: ""{EscapeGraphQLString(namespaceValue)}"",
                    key: ""{EscapeGraphQLString(key)}"",
                    description: ""{EscapeGraphQLString(description)}"",
                    type: ""{EscapeGraphQLString(type)}"",
                    ownerType: {ownerType},
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

            var variables = new
            {
                definition = new
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

            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("X-Shopify-Access-Token", accessToken);

            var requestBody = new
            {
                query = mutation,
                variables = variables
            };

            var content = new StringContent(JsonSerializer.Serialize(requestBody),
                encoding: Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(
                $"https://{shopDomain}/admin/api/2024-04/graphql.json", content);
            var responseString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("GraphQL Error: " +
                    $"{response.StatusCode} - {responseString}");
            };

            var jsonDoc = JsonDocument.Parse(responseString);
            var root = jsonDoc.RootElement;

            if (root.TryGetProperty("errors", out var errors))
            {
                _logger.LogInformation($"==============RESPONSE: {responseString}=============");

                var errorList = new List<string>();

                foreach (var error in errors.EnumerateArray())
                {
                    _logger.LogError($"Shopify GraphQL error: {error}");
                    errorList.Add(error.ToString());
                }

                if (errorList.Count != 0)
                {
                    throw new Exception("GraphQL error list: \n " +
                        $"{string.Join("\n", errorList)}");
                }
            }

            _logger.LogInformation($"Metafield definition created {responseString}");
        }

        public async Task EnsureMetafieldExistsAsync(string shopDomain, string accessToken,
                                             string namespaceVal, string key,
                                             string type, string ownerType,
                                             string name, string description,
                                             bool storeFrontVisibility)
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

            var variables = new
            {
                first = 100,
                ownerType = ownerType
            };

            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("X-Shopify-Access-Token", accessToken);

            var requestBody = new
            {
                query,
                variables
            };

            var content = new StringContent(JsonSerializer.Serialize(requestBody),
                Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(
                $"https://{shopDomain}/admin/api/2024-04/graphql.json", content);

            var responseString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"GraphQL error: {responseString}");
                throw new Exception($"GraphQL error while checking metafield definitions - {responseString}");
            }

            var jsonDoc = JsonDocument.Parse(responseString);
            var root = jsonDoc.RootElement;

            _logger.LogInformation(responseString);

            var edges = root.GetProperty("data")
                            .GetProperty("metafieldDefinitions")
                            .GetProperty("edges");

            _logger.LogInformation($"EDGES: {edges}");

            foreach (var edge in edges.EnumerateArray())
            {
                _logger.LogInformation($"EDGE: {edge}");
                var node = edge.GetProperty("node");
                var nodeNamespace = node.GetProperty("namespace").GetString();
                var nodeKey = node.GetProperty("key").GetString();

                if (nodeNamespace == namespaceVal && nodeKey == key)
                {
                    _logger.LogInformation($"Metafield definition already exists: {namespaceVal}.{key}" +
                        $" (Name: {node.GetProperty("name").GetString()})");
                    return;
                }
            }

            _logger.LogInformation($"Metafield definition not found, creating {namespaceVal}.{key}");

            await CreateMetafieldDefinitionAsync(shopDomain, accessToken, name,
                namespaceVal, key, type, ownerType, description, storeFrontVisibility);
        }
    }
}
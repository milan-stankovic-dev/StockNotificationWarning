using ShopifySharp.GraphQL;

namespace StockNotificationWarning.Dto
{
    public class CreateMetaobjectDefinitionResponse
    {
        public MetaobjectDefinition MetaobjectDefinition { get; set; }
        public List<UserError> UserErrors { get; set; }
    }
}

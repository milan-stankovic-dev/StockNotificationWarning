namespace StockNotificationWarning.Dto
{
    public class MetaobjectCreatePayload
    {
        public MetaobjectNode Metaobject { get; set; } = null!;
        public List<UserError> UserErrors { get; set; } = [];
    }
}

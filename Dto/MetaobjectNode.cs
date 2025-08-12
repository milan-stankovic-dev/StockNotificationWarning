namespace StockNotificationWarning.Dto
{
    public class MetaobjectNode
    {
        public string Id { get; set; } = null!;
        public string Handle { get; set; } = null!;
        public List<MetaobjectField> Fields { get; set; } = [];
    }
}

namespace StockNotificationWarning.Services.Abstraction
{
    public interface IShopTokenProvider
    {
        (string shop, string token) Provide();
    }
}

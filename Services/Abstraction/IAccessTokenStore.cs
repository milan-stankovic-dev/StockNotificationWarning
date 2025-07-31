namespace StockNotificationWarning.Services.Abstraction
{
    public interface IAccessTokenStore
    {
        void Set(string shop, string token);
        string? Get(string shop);
    }
}

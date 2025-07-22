namespace StockNotificationWarning.Services.Abstraction
{
    public interface IToastNotificationService
    {
        void AddToast(string toastMessage);
        IEnumerable<string> GetAllToasts();
        void ClearWarnings();
    }
}

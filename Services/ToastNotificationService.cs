using StockNotificationWarning.Services.Abstraction;
using System.Collections.Concurrent;

namespace StockNotificationWarning.Services
{
    public class ToastNotificationService : IToastNotificationService
    {
        readonly ConcurrentQueue<string> _toasts = new();
        public void AddToast(string toastMessage) => _toasts.Enqueue(toastMessage);
        public void ClearWarnings() => _toasts.Clear();
        public IEnumerable<string> GetAllToasts() => [.. _toasts];
    }
}

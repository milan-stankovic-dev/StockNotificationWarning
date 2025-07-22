using Microsoft.Extensions.Options;
using StockNotificationWarning.Config;
using StockNotificationWarning.Services.Abstraction;

namespace StockNotificationWarning.Services
{
    public class CronConfigProvider(IOptionsMonitor<CronConfig> options) : ICronConfigProvider
    {
        readonly int _cronDelay = options.CurrentValue.MinuteDelay;
        public int Provide() => _cronDelay;
    }
}

namespace StockNotificationWarning.Dto
{
    public class UnderstockedProductDto
    {
        public string ProductName { get; set; } = "";
        public long? Stock { get; set; } = 0;
    }
}

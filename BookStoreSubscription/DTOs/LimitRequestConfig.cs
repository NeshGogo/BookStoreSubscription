namespace BookStoreSubscription.DTOs
{
    public class LimitRequestConfig
    {
        public int RequestsByDayFree { get; set; }
        public string[] WhiteListRoutes { get; set; }
    }
}

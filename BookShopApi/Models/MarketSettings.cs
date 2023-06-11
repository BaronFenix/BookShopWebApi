namespace BookShopApi.Models
{

    public class MarketSettings
    {
        public string ShopName { get; set; }
        public PriceSettings PriceSettings { get; set; }
    }

    public class PriceSettings
    {
        public int MinPrice { get; set; }
        public int MaxPrice { get; set; }
    }
}

using Microsoft.Extensions.Options;
using BookShopApi.Models;
using System.Text;

namespace BookShopApi.Middleware
{
    public class MarketSettingsMiddleware
    {
        private readonly RequestDelegate _next;
        public MarketSettings MarketSettings { get; }

        public MarketSettingsMiddleware(RequestDelegate next, IOptions<MarketSettings> option)
        {
            _next = next;
            MarketSettings = option.Value;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            StringBuilder stringBuilder = new StringBuilder();

        }
    }
}

using System.Collections.Concurrent;
using System.Threading.Channels;

namespace BookShopApi.Middleware
{
    public class RateLimitMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ConcurrentDictionary<string, RateLimiter> _rateLimiters;

        public RateLimitMiddleware(RequestDelegate next)
        {
            _next = next;
            _rateLimiters = new ConcurrentDictionary<string, RateLimiter>();
        }

        public async Task Invoke(HttpContext context)
        {
            var ipAddress = context.Connection.RemoteIpAddress.ToString();
            var rateLimiter = _rateLimiters.GetOrAdd(ipAddress, new RateLimiter(20, TimeSpan.FromMinutes(1)));

            if (!rateLimiter.AllowRequest())
            {
                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                await context.Response.WriteAsync("Too many requests. Please try again later");
                return;
            }

            await _next(context);
        }

    }

    public static class RateLimitMiddlewareExtensions
    {
        public static IApplicationBuilder UseRateLimit(this IApplicationBuilder app)
        {
            return app.UseMiddleware<RateLimitMiddleware>();
        }
    }

    public class RateLimiter
    {
        private readonly int _maxRequests;
        private readonly TimeSpan _interval;
        private readonly object _lock = new object();
        private DateTime _startTime;
        private int _requestCount;


        public RateLimiter(int maxRequests, TimeSpan interval)
        {
            _maxRequests = maxRequests;
            _interval = interval;
            _startTime = DateTime.UtcNow;
            _requestCount = 0;
        }

        public bool AllowRequest()
        {
            lock (_lock)
            {
                var currentTime = DateTime.UtcNow;
                if (currentTime >= _startTime.Add(_interval))
                {
                    _startTime = currentTime;
                    _requestCount = 0;
                }
                if (_requestCount >= _maxRequests)
                {
                    return false;
                }
                _requestCount++;
                return true;
            }
        }



    }
}

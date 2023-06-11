using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Net.NetworkInformation;

namespace BookShopApi.Middleware
{
    public class PingHealthCheck : IHealthCheck
    {
        private string _host;
        private int _timemout;

        public PingHealthCheck(string host, int timeout)
        {
            _host = host;
            _timemout = timeout;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                using var ping = new Ping();
                var reply = await ping.SendPingAsync(_host, _timemout);

                return reply.Status != IPStatus.Success ? HealthCheckResult.Unhealthy($"{reply.RoundtripTime} ms.") :
                        reply.RoundtripTime >= _timemout ? HealthCheckResult.Degraded($"{reply.RoundtripTime} ms.") :
                                                            HealthCheckResult.Healthy($"{reply.RoundtripTime} ms.");
            }
            catch
            {
                return HealthCheckResult.Unhealthy();
            }
        }
    }
}

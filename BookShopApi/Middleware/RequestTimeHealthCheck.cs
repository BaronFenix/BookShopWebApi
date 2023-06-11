using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Diagnostics;

namespace BookShopApi.Middleware
{
    public class RequestTimeHealthCheck : IHealthCheck
    {
        int degraded_lvl = 2000;
        int unhealthy_lvl = 5000;
        HttpClient httpClient;

        public RequestTimeHealthCheck(HttpClient client) => httpClient = client;

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken token = default)
        {
            // Получаем время запроса
            Stopwatch sw = Stopwatch.StartNew();
            await httpClient.GetAsync("https://localhost:7087/Home/NewBooks");
            sw.Stop();

            var responceTime = sw.ElapsedMilliseconds;

            // В зависимости от времени запроса возвращаем определенный результат
            if (responceTime < degraded_lvl)
            {
                //return HealthCheckResult.Healthy("Система функционирует хорошо");
                return await Task.FromResult(HealthCheckResult.Healthy("Все отлично"));
            }
            else if (responceTime < unhealthy_lvl)
            {
                //return HealthCheckResult.Degraded("Снижение качества работы системы");
                return HealthCheckResult.Degraded("Могло быть и лучше");
            }
            else
            {
                //return HealthCheckResult.Unhealthy("Система в нерабочем состоянии. необходимо ее перезапустить");
                return HealthCheckResult.Unhealthy("Все плохо");
            }
        }

         

        //private HealthCheckResult Response(int responceTime)
        //{
        //}
    }
}

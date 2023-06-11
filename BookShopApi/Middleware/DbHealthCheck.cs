using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Data.SqlClient;

namespace BookShopApi.Middleware
{
    public class DbHealthCheck : IHealthCheck
    {
        private readonly string _connectionString;

        public DbHealthCheck(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                try
                {
                    await connection.OpenAsync(cancellationToken);

                    return HealthCheckResult.Healthy("MSSQL connection is healthy");
                }
                catch(Exception ex)
                {
                    return HealthCheckResult.Unhealthy("Faled to connect to MSSQL", ex);
                }
            }
        }

    }
}

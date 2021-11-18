using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;

namespace Pet.Api.Healthy
{
    public class ExternalServiceHealthCheck : IHealthCheck
    {
        private string _host;

        public ExternalServiceHealthCheck(string host)
        {
            _host = host;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context, 
            CancellationToken cancellationToken = default
        )
        {
            try
            {
                using Ping ping = new Ping();
                PingReply reply = await ping.SendPingAsync(_host);

                if (reply.Status != IPStatus.Success)
                    return HealthCheckResult.Unhealthy("Conexión fallida");

                if (reply.RoundtripTime > 100)
                    return HealthCheckResult.Degraded("Conexión lenta");

                return HealthCheckResult.Healthy("Conexión estable");
            }
            catch
            {
                return HealthCheckResult.Unhealthy();
            }
        }
    }
}

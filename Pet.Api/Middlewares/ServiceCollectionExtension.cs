using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Pet.Api.Healthy;

namespace Pet.Api.Middlewares
{
    public static class ServiceCollectionExtension
    {
        public static IHealthChecksBuilder ConfigureHealthChecks(this IServiceCollection services, IConfiguration configuration)
        {
            IHealthChecksBuilder healthChecksBuilder = services.AddHealthChecks();
            IConfigurationSection section = configuration.GetSection("HealthChecks:Services");

            foreach (IConfigurationSection sectionItem in section.GetChildren())
            {
                string name = sectionItem.GetValue<string>("Name");
                string host = sectionItem.GetValue<string>("Host");
                var tags = sectionItem.GetSection("Tags").Get<string[]>();

                healthChecksBuilder.AddTypeActivatedCheck<ExternalServiceHealthCheck>(
                    name, 
                    failureStatus: HealthStatus.Healthy, 
                    tags: tags, 
                    args: new object[] { host });
            }

            return healthChecksBuilder;
        }
    }
}

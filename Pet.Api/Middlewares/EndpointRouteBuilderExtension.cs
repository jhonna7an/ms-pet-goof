using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;
using System.Linq;

namespace Pet.Api.Middlewares
{
    public static class EndpointRouteBuilderExtension
    {
        public static void MapHealthChecksWithJsonResponse(this IEndpointRouteBuilder endpoints, PathString path, string tag)
        {
            HealthCheckOptions options = new HealthCheckOptions
            {
                Predicate = check => check.Tags.Contains(tag),
                ResponseWriter = async (httpContext, healthReport) =>
                {
                    httpContext.Response.ContentType = "application/json";

                    string result = JsonConvert.SerializeObject(new
                    {
                        status = healthReport.Status.ToString(),
                        duration = healthReport.TotalDuration.TotalSeconds,
                        services = healthReport.Entries
                            .Select(e => new
                            {
                                name = e.Key,
                                status = e.Value.Status.ToString(),
                                description = e.Value.Description,
                                duration = e.Value.Duration.TotalSeconds,
                                Tags = e.Value.Tags.ToArray()
                            })
                    });

                    await httpContext.Response.WriteAsync(result);
                }
            };

            endpoints.MapHealthChecks(path, options);
        }
    }
}

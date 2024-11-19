using OpenTelemetry.Metrics;
using OpenTelemetry.ResourceDetectors.Container;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace DemoApi.Extensions;

public static class OpenTelemetryExtensions
{
    public static WebApplicationBuilder ConfigureOpenTelemetry(this WebApplicationBuilder builder)
    {
        Action<ResourceBuilder> appResourceBuilder =
            resource => resource
                .AddDetector(new ContainerResourceDetector());

        builder.Services.AddOpenTelemetry()
            .ConfigureResource(appResourceBuilder)
            .WithTracing(tracerBuilder => tracerBuilder
                .SetSampler(new AlwaysOnSampler())
                .AddSource("DemoApi.Services.Delay")
                .AddSource("DemoApi.Services.Random")
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddOtlpExporter()
            )
            .WithMetrics(meterBuilder =>
            {
                meterBuilder.AddPrometheusExporter();
                meterBuilder.AddAspNetCoreInstrumentation();
                meterBuilder.AddHttpClientInstrumentation();
                meterBuilder.AddMeter(
                    "Microsoft:AspNetCore.Hosting",
                    "Microsoft.AspNetCore.Server.Kestrel",
                    "System.Net.Http");
                meterBuilder.AddView("http.server.request.duration",
                    new ExplicitBucketHistogramConfiguration
                    {
                        Boundaries = new double[]
                        {
                            0, 0.005, 0.01, 0.025, 0.05,
                            0.075, 0.1, 0.25, 0.5, 0.75, 1, 2.5, 5, 7.5, 10
                        }
                    });
                meterBuilder.AddView("http.server.active_requests",
                    new ExplicitBucketHistogramConfiguration
                    {
                        Boundaries = new double[]
                        {
                            0, 0.005, 0.01, 0.025, 0.05,
                            0.075, 0.1, 0.25, 0.5, 0.75, 1, 2.5, 5, 7.5, 10
                        }
                    });
            });

        return builder;
    }
}
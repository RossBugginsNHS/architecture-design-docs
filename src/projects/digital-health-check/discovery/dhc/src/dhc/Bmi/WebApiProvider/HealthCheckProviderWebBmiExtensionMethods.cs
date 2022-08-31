using System.Net;
using Prometheus.HttpClientMetrics;

namespace dhc;

public static class HealthCheckProviderBmiWebApiProviderExtensionMethods
{


    public static HealthCheckProviderOptions AddWebBmiProvider(this HealthCheckProviderOptions options, IConfiguration config)
    {
        var settings = new BmiWebApiSettings();
        config.GetSection(BmiWebApiSettings.Position).Bind(settings);
        if (!string.IsNullOrWhiteSpace(settings.BaseUrl))
        {
            options.Services.Configure<BmiWebApiSettings>(config.GetSection(BmiWebApiSettings.Position));
            options.SetBmiProvider<WebBmiProvider>();
            options.Services.AddTransient(typeof(TimeRequestHeaderHandler<>));

            options.Services.AddHttpClient<bmiclient.IBmiClient, bmiclient.BmiClient>(
                (sp, c) =>
                {
                    c.BaseAddress = new Uri(settings.BaseUrl);
                })
                .SetHandlerLifetime(TimeSpan.FromMinutes(5))  //Set lifetime to five minutes
                .AddPolicyHandler((sp, m) =>
                    {
                        return HttpClientRetryPolicies.GetRetryPolicy<bmiclient.BmiClient>(sp);
                    })
                .AddTimerAndLoggerHandler<bmiclient.BmiClient>()
                .UseHttpClientMetrics(o =>
                {
                    o.InProgress.Enabled = true;
                    o.RequestCount.Enabled = true;
                    o.RequestDuration.Enabled = true;
                    o.ResponseDuration.Enabled = true;
                });
        }
        return options;
    }

}

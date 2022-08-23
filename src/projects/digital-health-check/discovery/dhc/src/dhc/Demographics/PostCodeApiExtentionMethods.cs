namespace dhc;

public static class PostCodeApiExtentionMethods
{
    public static HealthCheckProviderOptions AddPostCodeApi(this HealthCheckProviderOptions options, IConfiguration config)
    {
        var settings = new PostCodeApiSettings();
        config.GetSection(PostCodeApiSettings.Location).Bind(settings);
        options.Services.Configure<PostCodeApiSettings>(config.GetSection(PostCodeApiSettings.Location));

        options.Services.AddHttpClient<PostCodeHttpClient>(
            (sp, c) =>
            {
                   c.BaseAddress = new Uri(settings.BaseUrl);
            })
            .SetHandlerLifetime(TimeSpan.FromMinutes(5))  //Set lifetime to five minutes
            .AddPolicyHandler((sp, m) =>
                {
                    return HttpClientRetryPolicies.GetRetryPolicy<PostCodeHttpClient>(sp);
                })
            .AddTimerAndLoggerHandler<bmiclient.BmiClient>()
            .UseHttpClientMetrics(o =>
            {
                o.InProgress.Enabled = true;
                o.RequestCount.Enabled = true;
                o.RequestDuration.Enabled = true;
                o.ResponseDuration.Enabled = true;
            });

        return options;
    }

}
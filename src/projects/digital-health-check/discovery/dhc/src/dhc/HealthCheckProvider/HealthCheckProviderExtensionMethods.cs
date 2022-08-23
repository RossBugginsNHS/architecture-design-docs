using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using MediatR;
using FluentValidation;
namespace dhc;


public static class HealthCheckProviderExtensionMethods
{
    public static IServiceCollection AddHealthCheck(this IServiceCollection services)
    {
        return AddHealthCheck(services, (o) => { });
    }

    public static IServiceCollection AddHealthCheck(this IServiceCollection services, Action<HealthCheckProviderOptions> configuration)
    {
        var options = HealthCheckProviderOptionsDefaults.Defaults(services);
        configuration(options);

        return services
            .AddHealthCheckProviderFilters(options)
            .AddHealthCheckProviderGuidanceFilters(options)
            .AddHealthCheckHealthCheckDataBuilders(options)
            .AddOtherRequirements(options)
            .AddBmiProvider(options)
            .AddMediatR(typeof(HealthCheckProvider))
            .AddValidatorsFromAssemblyContaining<HealthCheckProvider>();
    }

    private static IServiceCollection AddOtherRequirements(this IServiceCollection services, HealthCheckProviderOptions options)
    {
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehaviour<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
        services.AddTransient<IHealthCheckDataBuilder, HealthCheckDataBuilder>();
        services.AddTransient<IHealthCheckDataBuilderProvider, HealthCheckDataBuilderProvider>();
        services.AddTransient<IBloodPressureProvider, BloodPressureProvider>();
        services.AddTransient<IBmiCalculatorProvider, BmiCalculatorProvider>();
        services.AddTransient<IHealthCheckProvider, HealthCheckProvider>();
        services.AddTransient<SmokingCalculator>();
        services.AddTransient(typeof(IPipelineBuilder<,>), typeof(PipelineBuilder<,>));
        services.AddSingleton(typeof(IPipelineRunner<,>), typeof(PipelineRunner<,>));

        services.AddTransient<IHealthCheckContextFactory, HealthCheckContextFactory>();
        services.AddTransient<IHealthCheckContext, HealthCheckContext>();
        services.AddSingleton(typeof(IContextHandlerFactory<>), typeof(ContextHandlerFactory<>));

        //Used to pre load the Singleton IPipelinerunner - not actually needed, but makes first run quicker.
        services.AddHostedService<SetupPipelineHostedService<IHealthCheckProviderFilter, IHealthCheckContext>>();

        services.AddTransient<ICholesterolCalculatorProvider, CholesterolCalculatorProvider>();

        services.AddHttpClient();

        services.AddHostedService<LoadDataHostedService>();
        services.AddSingleton<TdsDataProvider>();
        services.AddTransient<QRiskProvider>();
        services.AddTransient<DemographicsProvider>();

        return services;
    }



    private static IServiceCollection AddBmiProvider(this IServiceCollection services, HealthCheckProviderOptions options)
    {
        return services.AddTransient(typeof(IBmiCalculatorProvider), options.BmiProvider);
    }

    private static IServiceCollection AddHealthCheckProviderFilters(this IServiceCollection services, HealthCheckProviderOptions options)
    {
        return services.AddTransientTypesFrom<IHealthCheckProviderFilter>(options.Filters.Types);
    }

    private static IServiceCollection AddHealthCheckProviderGuidanceFilters(this IServiceCollection services, HealthCheckProviderOptions options)
    {
        return services.AddTransientTypesFrom<IHealthCheckProviderFilter>(options.GuidanceFilters.Types);
    }

    private static IServiceCollection AddHealthCheckHealthCheckDataBuilders(this IServiceCollection services, HealthCheckProviderOptions options)
    {
        return services.AddTransientTypesFrom<IHealthCheckDataBuilderBuildFilter>(options.HealthCheckDataBuilders.Types);
    }


    private static IServiceCollection AddTransientTypesFrom<T>(this IServiceCollection services, IEnumerable<Type> types)
    {
        foreach (var t in types)
        {
            services.AddTransient(typeof(T), t);
        }

        return services;
    }
}

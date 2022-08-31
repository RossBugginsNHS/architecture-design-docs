using Microsoft.Extensions.DependencyInjection;

namespace dhc;

public class HealthCheckProviderOptions
{
    private Type _bmiProvider;

    public Type BmiProvider {get{return _bmiProvider;}}

        private Type _bpProvider;

    public Type BpProvider {get{return _bpProvider;}}

    public HealthCheckProviderOptions(IServiceCollection services)
    {
        Services = services;
        Filters = new HealthCheckProviderFilterOptions(services, this);
        GuidanceFilters = new HealthCheckProviderGuidanceFilterOptions(services, this);
        HealthCheckDataBuilders = new HealthCheckDataBuilderOptions(services, this);
    }

    public void SetBmiProvider<T>() where T : IBmiCalculatorProvider
    {
        _bmiProvider = typeof(T);
    }

        public void SetBpProvider<T>() where T : IBloodPressureProvider
    {
        _bpProvider = typeof(T);
    }


    public IServiceCollection Services {get; init;}
    public HealthCheckProviderFilterOptions Filters{get; init;}
    public HealthCheckProviderGuidanceFilterOptions GuidanceFilters{get; init;}
    public HealthCheckDataBuilderOptions HealthCheckDataBuilders{get;init;}

}
using Orleans;

namespace dhc;
    public interface IHealthCheckGrain : Orleans.IGrainWithGuidKey
    {
        Task AddData(HealthCheckData data);
        Task Calculate(GrainCancellationToken cancellationToken);
    }

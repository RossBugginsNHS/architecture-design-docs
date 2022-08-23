namespace dhc;

using System.Threading;
using System.Threading.Tasks;
using MediatR;

public class CalculateHealthCheckCommandHandler : IRequestHandler<CalculateHealthCheckCommand, HealthCheckResult>
{

    private readonly IHealthCheckProvider _healthCheckProvider;
    private readonly IPublisher _publisher;

    public CalculateHealthCheckCommandHandler(
        IHealthCheckProvider healthCheckProvider,
        IPublisher publisher)
    {
        _healthCheckProvider = healthCheckProvider;
        _publisher = publisher;
    }

    public async Task<HealthCheckResult> Handle(CalculateHealthCheckCommand request, CancellationToken cancellationToken)
    {
        var result = await _healthCheckProvider.CalculateAsync(request.HealthCheckData);
        await _publisher.Publish(new CalculateHealthCheckCommandHandledNotification(request.HealthCheckData, result));
        return result;
    }
}

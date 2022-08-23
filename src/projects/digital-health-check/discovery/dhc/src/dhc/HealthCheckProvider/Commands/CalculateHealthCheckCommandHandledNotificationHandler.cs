namespace dhc;

using System.Threading;
using System.Threading.Tasks;
using MediatR;

public class CalculateHealthCheckCommandHandledNotificationHandler : INotificationHandler<CalculateHealthCheckCommandHandledNotification>
{
    private static readonly Counter _c_get_health_check = Metrics.CreateCounter("healthcheck_completed_counter", "Health Check Completed");     

    public Task Handle(CalculateHealthCheckCommandHandledNotification notification, CancellationToken cancellationToken)
    {
        _c_get_health_check.Inc();
        return Task.CompletedTask;
    }
}
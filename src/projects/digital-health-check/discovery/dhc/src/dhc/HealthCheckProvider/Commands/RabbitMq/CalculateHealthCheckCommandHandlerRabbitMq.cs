namespace dhc;

using System.Threading;
using System.Threading.Tasks;
using MediatR;
using RabbitMQ.Client;

public class CalculateHealthCheckCommandHandlerRabbitMq : IRequestHandler<CalculateHealthCheckCommand, HealthCheckResult>
{
    RabbitMqClient _client;
    private readonly IPublisher _publisher;



    public CalculateHealthCheckCommandHandlerRabbitMq(RabbitMqClient client )
    {
        _client = client;
    }
    private static readonly Counter _c_get_health_check = Metrics.CreateCounter("healthcheck_completed_counter", "Health Check Completed");    

    public async  Task<HealthCheckResult> Handle(CalculateHealthCheckCommand request, CancellationToken cancellationToken)
    {
        _client.PushMessage("dhc.healthchecks", request.HealthCheckData.HealthCheckDataId.id.ToString(), request.HealthCheckData);
        var result = default(HealthCheckResult);

        return result;
    }
}
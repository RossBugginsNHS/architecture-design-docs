using MediatR;

namespace bpapi;

public class CalculateBloodPressureCommandHandler: IRequestHandler<CalculateBloodPressureCommand, BloodPressure>
{
    private readonly IBloodPressureProvider _bpProvider;
    public CalculateBloodPressureCommandHandler(IBloodPressureProvider bpProvider)
    {
        _bpProvider = bpProvider;
    }
    public Task<BloodPressure> Handle(CalculateBloodPressureCommand request, CancellationToken cancellationToken)
    {
        var obs = request.RequestData;
        var bpResult = _bpProvider.CalculateBloodPressure(obs.Systolic,obs.Diastolic);
        return Task.FromResult(bpResult);
    }
}

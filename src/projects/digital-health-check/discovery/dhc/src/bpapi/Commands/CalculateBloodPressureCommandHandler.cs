using MediatR;

namespace bpapi;

public class CalculateBloodPressureCommandHandler: IRequestHandler<CalculateBloodPressureCommand, BloodPressure>
{
    private readonly IBloodPressureProvider _bpProvider;
    public CalculateBloodPressureCommandHandler(IBloodPressureProvider bpProvider)
    {
        _bpProvider = bpProvider;
    }
    public async Task<BloodPressure> Handle(CalculateBloodPressureCommand request, CancellationToken cancellationToken)
    {
        var obs = request.RequestData;
        var bpResult = await _bpProvider.CalculateBloodPressure(obs.Systolic,obs.Diastolic, cancellationToken);
        return bpResult;
    }
}

using MediatR;
namespace bpapi;

public record CalculateBloodPressureCommand(BloodPressureObservation RequestData):  IRequest<BloodPressure>;


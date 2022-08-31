namespace dhc;

public readonly record struct HealthCheckData(HealthCheckDataId HealthCheckDataId, HealthCheckDemographicData Demographics, HealthCheckBasicObsData BasicObs, BloodPressureObservation BloodPressure, SmokingData SmokingData, CholesterolObservation CholesterolData);

public readonly record struct HealthCheckDataId(Guid id);
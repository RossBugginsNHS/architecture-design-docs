namespace dhc;

public readonly record struct HealthCheckDemographicData(Age Age, Postcode Postcode,SexAtBirthEnum SexAtBirthEnum);

public enum SexAtBirthEnum
{
    Male,
    Female
}
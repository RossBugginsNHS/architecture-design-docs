using dhc;


namespace dhc;
public class QRiskProvider
{
 
    public async Task CalculateAsync(HealthCheckData data, HealthCheckResult results)
    {
        if(data.Demographics.SexAtBirthEnum == SexAtBirthEnum.Female)
        {
            var years = (int)Math.Floor(data.Demographics.Age.Days / (double) 365);
            var qRisk = QRisk3.cvd_female_raw(years, 0,0,0,0,0,0,0,0,0,0,0,(double)results.Bmi.BmiValue, 0,0,0, results.BloodPressure.Systolic, 0,0,10,results.Demographics.Tds);

            
        }
    }
}
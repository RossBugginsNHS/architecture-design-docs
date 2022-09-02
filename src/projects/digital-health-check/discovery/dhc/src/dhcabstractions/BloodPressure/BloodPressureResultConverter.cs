namespace dhc;

public static class BloodPressureResultConverter
{
    public static string GetResult(double systolic, double diastolic) =>
        (systolic, diastolic) switch
        {
            _ when systolic < diastolic => "Something might be wrong with the data, is it the correct way round?",

            (<90, <60) => "Low",
            (>=140, >=90) => "High",
            (>=120, >=80) => "Above Normal",

            (<90, _) => "Low on top",
            (_, <60) => "Low on bottom",

            (>=140, _) => "High on top",
            (_, >=90) => "High on bottom",

            (>=120,_) => "Above Normal on top",
            (_, >=80) => "Above Normal  on bottom",

            ((>=90 and <120), (>=60 and < 80)) => "Normal",


            (_,_) => "Error"
        };
}

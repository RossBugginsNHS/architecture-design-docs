
public interface ICholesterolCalculatorProvider
{
    Task<CholesterolResult> Calculate(CholesterolObservation obs);
}
// namespace tests;

// public class BmiProviderTests
// {

//     [Fact]
//     public void TestBmi()
//     {
//         //arrange
//         var mass = Mass.FromKilograms(100);
//         var height = Length.FromCentimeters(180);
//         var consoleLogger = LoggerFactory.Create(loggingBuilder => loggingBuilder
//             .AddConsole()
//             .AddDebug()
//             .SetMinimumLevel(LogLevel.Trace));
            
//         var logger = consoleLogger.CreateLogger<BmiCalculatorProvider>();
//         var calculator = new BmiCalculatorProvider(logger);
//         //act
//         var bmi = calculator.CalculateBmi(height, mass);

//         //assert
//         var result = bmi.BmiValue;
//         var expected =  30.9m;

//         Assert.Equal(expected, result);
//     }     
// }
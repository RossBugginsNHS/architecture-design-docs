// namespace tests;

// public class BmiTests
// {
//     [Fact]
//     public void TestBmi30Obese()
//     {
//         //arrange
//         var bmiNumber = 30;

//         //act
//         var bmi = new Bmi(bmiNumber);

//         //assert
//         var result = bmi.BmiDescription;
//         var expected =  BmiEnum.Obese;

//         Assert.Equal(expected, result);
//     }       

//     [Fact]
//     public void TestBmi23Healthy()
//     {
//         //arrange
//         var bmiNumber = 23;

//         //act
//         var bmi = new Bmi(bmiNumber);

//         //assert
//         var result = bmi.BmiDescription;
//         var expected =  BmiEnum.Healthy;

//         Assert.Equal(expected, result);
//     }       

//     [Fact]
//     public void TestBmi19Underweight()
//     {
//         //arrange
//         var bmiNumber = 18;

//         //act
//         var bmi = new Bmi(bmiNumber);
//         //assert
//         var result = bmi.BmiDescription;
//         var expected =  BmiEnum.Underweight;

//         Assert.Equal(expected, result);
//     }        

//     [Fact]
//     public void TestBmi28Overweight()
//     {
//         //arrange
//         var bmiNumber = 28;

//         //act
//         var bmi = new Bmi(bmiNumber);
        
//         //assert
//         var result = bmi.BmiDescription;
//         var expected =  BmiEnum.Overweight;

//         Assert.Equal(expected, result);
//     }  

//     [Fact]
//     public void TestBmi41EObese()
//     {
//         //arrange
//         var bmiNumber = 41;

//         //act
//         var bmi = new Bmi(bmiNumber);

//         //assert
//         var result = bmi.BmiDescription;
//         var expected =  BmiEnum.EObese;

//         Assert.Equal(expected, result);
//     }               
// }
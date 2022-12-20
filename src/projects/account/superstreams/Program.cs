using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

try
{
    await using var consumersGroup1 = new SuperConsumer(){NumberOfClients=5};
    await using var producer = new SuperProducer();
    var proder = producer.Connect();
    await consumersGroup1.Connect();
    await proder;
    Console.ReadKey();
}

catch (Exception ex)
{
    Console.WriteLine("Error:\t" + ex.Message);
}
finally
{

}

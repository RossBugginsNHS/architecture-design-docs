using Orleans.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using account;
using MassTransit;
using account.api;

try
{
    var host = await StartClientAsync();
    var client = host.Services.GetRequiredService<IClusterClient>();
    Console.ReadKey();    

    return 0;
}
catch (Exception e)
{
    Console.WriteLine($"\nException while trying to run client: {e.Message}");
    Console.WriteLine("Make sure the silo the client is trying to connect to is running.");
    Console.WriteLine("\nPress any key to exit.");
    Console.ReadKey();
    return 1;
}

static async Task<IHost> StartClientAsync()
{
    var builder = new HostBuilder()
        .UseOrleansClient(client => 
        {
            client.UseLocalhostClustering()
                .Configure<ClusterOptions>(options => 
                {
                    options.ClusterId = "dev";
                    options.ServiceId = "OrleansBasics";
                });
        })            
        .ConfigureLogging(logging => logging.AddConsole());

    builder.ConfigureServices(services =>
    {
        services.AddMassTransit(x =>
        {
            x.AddConsumer<CreateNewAccountConsumer>(typeof(CreateNewAccountDefinition));
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host("localhost", "/", h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });
                cfg.ConfigureEndpoints(context);
            });
        });
    });

    var host = builder.Build();
    await host.StartAsync();
    
    Console.WriteLine("Client successfully connected to silo host \n");

    return host;
}

static async Task DoClientWorkAsync(IClusterClient client)
{
    var id = Guid.NewGuid();
    var nhsAccount = client.GetGrain<IAccount>(id);
    await nhsAccount.SetNhsNumber(new NHSNumber("123123123", DateOnly.FromDateTime(DateTime.Now)));
    var number = await nhsAccount.GetNhsNumber();
    Console.WriteLine($"Its {number.Number}");

    var r = await nhsAccount.AddRelationship(new Relationship(id, Guid.NewGuid(), "Parent", "MoJ"));
    Console.WriteLine($"Relationship:\t {r}");
 
}
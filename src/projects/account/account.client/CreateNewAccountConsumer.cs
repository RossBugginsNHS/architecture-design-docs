using MassTransit;
using Microsoft.Extensions.Logging;

namespace account.api
{
    public class CreateNewAccountConsumer :
        IConsumer<CreateNewAccount>
    {
        readonly ILogger<CreateNewAccountConsumer> _logger;
        readonly IClusterClient _client;

        public CreateNewAccountConsumer(
            IClusterClient client,
            ILogger<CreateNewAccountConsumer> logger)
        {
            _client = client;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<CreateNewAccount> context)
        {
            var id = context.Message.AccountId;
            var nhsAccount = _client.GetGrain<IAccount>(id);
            await nhsAccount.SetNhsNumber(
                new NHSNumber(context.Message.NHSNumber, DateOnly.FromDateTime(DateTime.Now)));
            var number = await nhsAccount.GetNhsNumber();
     

            _logger.LogInformation("Received Create Account for {account} to {nhsNumber}", id, context.Message.NHSNumber);
            
        }
    }
}
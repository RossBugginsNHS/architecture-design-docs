using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Orleans;
using EventStore.Client;
using System.Text.Json;

namespace account
{
    public class AccountGrain : Orleans.Grain, IAccount
    {
        private INHSNumber _nhsNumber;
        private readonly ILogger _logger;
        private readonly IList<IAccountUser> _accountUsers = new List<IAccountUser>();

        public AccountGrain(ILogger<AccountGrain> logger)
        {
            _logger = logger;
        }

        public async Task<INHSNumber> SetNhsNumber(INHSNumber nhsNumber)
        {
            var accountId = this.GetPrimaryKey();
            var connectionString = "esdb://localhost:2113?tls=false&tlsVerifyCert=false";
            var settings = EventStoreClientSettings
                .Create($"{connectionString}");
            var client = new EventStoreClient(settings);

            var eventData = new EventData(
                Uuid.NewUuid(),
                "NHSNumberSet",
                JsonSerializer.SerializeToUtf8Bytes(nhsNumber)
            );

            await client.AppendToStreamAsync(
                "account-"+accountId,
                StreamState.Any,
                new[] { eventData },
                cancellationToken: default(CancellationToken)
);

            _nhsNumber = nhsNumber;
            _logger.LogInformation("Set Nhs number for {account} to {nhsNumber}", accountId, nhsNumber.Number);
            return await GetNhsNumber();
        }

        public async Task<INHSNumber> GetNhsNumber()
        {
            await Task.Yield();
            return _nhsNumber;
        }

        public Task<Relationship> AddRelationship(Relationship relationship)
        {
            return Task.FromResult(relationship);
        }

        public Task<IEnumerable<IAccountUser>> GetAccountUsers()
        {
            return Task.FromResult(_accountUsers.AsEnumerable());
        }

        public Task<IAccountUser> AddAccountUser(IAccountUser user)
        {
            _accountUsers.Add(user);
            return Task.FromResult(user);
        }
    }
}
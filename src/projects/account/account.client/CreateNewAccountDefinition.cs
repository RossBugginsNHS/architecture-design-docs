using MassTransit;

namespace account.api
{
    public class CreateNewAccountDefinition :
    ConsumerDefinition<CreateNewAccountConsumer>
{
    public CreateNewAccountDefinition()
    {
        // override the default endpoint name, for whatever reason
        EndpointName = "create-new-account";

        // limit the number of messages consumed concurrently
        // this applies to the consumer only, not the endpoint
        ConcurrentMessageLimit = 1000;
    }


}
}
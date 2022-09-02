using System.Net;
using Microsoft.Extensions.Hosting;
using Orleans;
using Orleans.Configuration;
using Polly;
using Polly.Contrib.WaitAndRetry;
namespace dhc;

public class ClusterClientFactory
{
    private readonly SemaphoreSlim _slim = new  SemaphoreSlim(0, 1);
    private IClusterClient _client;

    public async Task<IClusterClient> GetClientAsync(TimeSpan timeout, CancellationToken cancellationToken)
    {
        if(_client!=null)
            return _client;

        CancellationTokenSource cts = new CancellationTokenSource(timeout);
        cancellationToken.Register(()=>cts.Cancel());
        var task = _slim.WaitAsync(cts.Token);
        try
        {
            await task;
        }
        catch(Exception ex)
        {
            throw;
        }
        if(task.IsCompletedSuccessfully)
        { 
            _slim.Release();
            return _client;
        }
        else
        {
            throw new Exception("Failed to get client in time.");
        }
    }

    public void SetClient(IClusterClient client)
    {
        if(client!=null)
        {
            _client = client;
            _slim.Release();
        }
    }

}
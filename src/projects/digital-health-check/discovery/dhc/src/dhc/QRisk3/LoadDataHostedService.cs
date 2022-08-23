using System.Buffers;
using System.Threading.Tasks.Dataflow;
using Microsoft.Extensions.Hosting;

namespace dhc;
public class LoadDataHostedService : IHostedService
{
    TdsDataProvider _provider;
    public LoadDataHostedService(TdsDataProvider provider)
    {
        _provider = provider;
    }
  

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var buffer = new BufferBlock<string>();
        var consumerTask = ConsumeAsync(buffer);
        var finishedProduce = Produce(buffer);
        await Task.WhenAll(finishedProduce,consumerTask);
    }

    private async Task Produce(ITargetBlock<string> target)
    {
                var fileName = "data.tsv";
        using (TextReader fileReader = File.OpenText(fileName))
        {
            string readLine = await fileReader.ReadLineAsync();
            readLine = await fileReader.ReadLineAsync();
            while (readLine != null)
            {
                // a line has been read; put it on the buffer:
                
                await target.SendAsync(readLine);

                // read the next line                
                readLine = await fileReader.ReadLineAsync();
            }

            target.Complete();
        }

    }

    private async Task ConsumeAsync(ISourceBlock<string> source)
    {
        int bytesProcessed = 0;

        while (await source.OutputAvailableAsync())
        {
            var rec = await source.ReceiveAsync();
            var split = rec.Split("\t");

            try
            {

            var d = new TdsData(int.Parse(split[0].Trim()), split[1].Trim(), split[2].Trim(), double.Parse(split[3].Trim()), double.Parse(split[4].Trim()));
            _provider.Data.Add(d.GeoCode, d);
            }
            catch(Exception ex)
            {

            }
        }

    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
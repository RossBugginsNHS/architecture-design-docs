using System.Buffers;
using System.Threading.Tasks.Dataflow;
using Microsoft.Extensions.Hosting;

namespace dhc;
public class LoadDataHostedService : IHostedService
{
     string fileName = "data.tsv";
    TdsDataProvider _provider;
    ILogger<LoadDataHostedService> _logger;
    public LoadDataHostedService(TdsDataProvider provider, ILogger<LoadDataHostedService> logger)
    {
        _provider = provider;
        _logger = logger;
    }


    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var buffer = new BufferBlock<string>();
        var consumerTask = ConsumeAsync(buffer);
        var finishedProduce = Produce(buffer);
        await Task.WhenAll(finishedProduce, consumerTask);
    }

    private async Task Produce(ITargetBlock<string> target)
    {
       
        var fi = new FileInfo(fileName);
        if (fi.Exists)
            _logger.LogInformation("Loading data from file {fileName}", fi.FullName);
        else
            _logger.LogError("Data file {fileName} does not exist.", fi.FullName);

        if (!fi.Exists)
        {
            target.Complete();
        }
        else
        {
            using (TextReader fileReader = File.OpenText(fileName))
            {
                string readLine = await fileReader.ReadLineAsync();
                readLine = await fileReader.ReadLineAsync();
                int count = 0;
                while (readLine != null)
                {
                    count++;
                    // a line has been read; put it on the buffer:

                    await target.SendAsync(readLine);

                    // read the next line                
                    readLine = await fileReader.ReadLineAsync();
                }
                target.Complete();

                 _logger.LogInformation("Loaded {count} data entries from file {fileName}", count, fi.FullName);
            }
        }

    }

    private async Task ConsumeAsync(ISourceBlock<string> source)
    {
        var fi = new FileInfo(fileName);
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
            catch (Exception ex)
            {
                 _logger.LogError("Failed parsing data row {dataRow} from {fileName}", rec, fi.FullName);
            }
        }

    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
//using System.IO;
//using System.Threading.Tasks;
//using Microsoft.Azure.Functions.Worker;
//using Microsoft.Extensions.Logging;

//namespace AppInsightsLoggingProviders
//{
//    public class blob1
//    {
//        private readonly ILogger<blob1> _logger;

//        public blob1(ILogger<blob1> logger)
//        {
//            _logger = logger;
//        }

//        [Function(nameof(blob1))]
//        public async Task Run([BlobTrigger("container1/{name}")] Stream stream, string name)
//        {
//            using var blobStreamReader = new StreamReader(stream);
//            var content = await blobStreamReader.ReadToEndAsync();
//            _logger.LogInformation($"C# Blob trigger function Processed blob\n Name: {name} \n Data: {content}");
//        }
//    }
//}

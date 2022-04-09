using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace Importer
{
    public class ImportFunction
    {
        [FunctionName("Function1")]
        public void Run([TimerTrigger("0 0 3 * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            DownloadFiles();
            ImportFiles();
            Report();
        }

        private void DownloadFiles()
        {

        }

        private void ImportFiles()
        {

        }

        private void Report()
        {

        }

    }
}
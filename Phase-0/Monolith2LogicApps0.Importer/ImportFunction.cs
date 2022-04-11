using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Monolith2LogicApps0.Application.CsvConversion;
using Monolith2LogicApps0.Domain;

namespace Importer
{
    public class ImportFunction
    {
        [FunctionName("Function1")]
        [StorageAccount("AzureWebJobsStorage")]
        public async Task Run(
            [TimerTrigger("0 0 3 * * *")] TimerInfo myTimer,
            [Blob("monolith2logicapps", FileAccess.ReadWrite)] BlobContainerClient blobContainerClient,
            [Sql("dbo.TypeAs", ConnectionStringSetting = "SqlConnection")] IAsyncCollector<TypeA> typeACollector,
            ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            DownloadFiles();
            var items = await ReadItems(blobContainerClient, log);
            if (!items.Any())
                return;
            await ImportTypeAFiles(items, typeACollector);
            Report();
        }

        private void DownloadFiles()
        {

        }

        private async Task<IEnumerable<TypeA>> ReadItems(
            BlobContainerClient blobContainerClient,
            ILogger log)
        {
            var items = new List<TypeA>();
            var reader = new CsvReader();
            var converter = new TypeACsvLineConverter();
            await foreach (var blob in blobContainerClient
                .GetBlobsAsync(
                Azure.Storage.Blobs.Models.BlobTraits.All,
                Azure.Storage.Blobs.Models.BlobStates.None,
                "TypeAs/"))
            {
                try
                {
                    var importedItems = reader.Parse(null, converter);
                    items.AddRange(items);
                }
                catch (Exception ex)
                {
                    log.LogError(ex, "Error doing import");
                }
            }
            return items;
        }

        private static async Task ImportTypeAFiles(
            IEnumerable<TypeA> items,
            IAsyncCollector<TypeA> typeACollector)
        {
            foreach (var item in items)
                await typeACollector.AddAsync(item);
        }

        private void Report()
        {

        }

    }
}
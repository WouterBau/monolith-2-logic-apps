using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using FluentFTP;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Monolith2LogicApps0.Application.CsvConversion;
using Monolith2LogicApps0.Domain;

namespace Importer
{
    public class ImportFunction
    {

        private const string LOCATION_NAME = "TypeA";

        [FunctionName("Function1")]
        [StorageAccount("AzureWebJobsStorage")]
        public async Task Run(
            [TimerTrigger("0 0 3 * * *")] TimerInfo myTimer,
            [Blob("import", FileAccess.ReadWrite, Connection = "Monolith2LogicAppsStorage")] BlobContainerClient blobContainerClient,
            [Sql("dbo.TypeAs", ConnectionStringSetting = "SqlConnection")] IAsyncCollector<TypeA> typeACollector,
            ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            await DownloadFiles(blobContainerClient, log);
            var items = await ReadItems(blobContainerClient, log);
            if (!items.Any())
                return;
            await ImportTypeAFiles(items, typeACollector);
            Report();
        }

        private async Task DownloadFiles(
            BlobContainerClient blobContainerClient,
            ILogger log)
        {
            using var client = new FtpClient("127.0.0.1", "monolith2logicapps", "");
            await client.AutoConnectAsync();
            foreach (var item in await client.GetListingAsync("/" + LOCATION_NAME))
            {
                if (item.Type != FtpFileSystemObjectType.File)
                    continue;

                var data = await client.DownloadAsync(item.FullName, token: default);
                await blobContainerClient.UploadBlobAsync(item.Name, new MemoryStream(data));
                log.LogInformation($"File {item.Name} uploaded to Blob Storage");
            }
        }

        private async Task<IEnumerable<TypeA>> ReadItems(
            BlobContainerClient blobContainerClient,
            ILogger log)
        {
            var items = new List<TypeA>();
            var reader = new CsvReader();
            var converter = new TypeACsvLineConverter();
            await foreach (var blobItem in blobContainerClient.GetBlobsAsync(Azure.Storage.Blobs.Models.BlobTraits.All,
                Azure.Storage.Blobs.Models.BlobStates.None, LOCATION_NAME + "/"))
            {
                try
                {
                    var blobClient = blobContainerClient.GetBlobClient(blobItem.Name);
                    var blobDownload = await blobClient.DownloadContentAsync();
                    var blobStream = blobDownload.Value.Content.ToStream();
                    var importedItems = reader.Parse(blobStream, converter);
                    items.AddRange(importedItems);
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
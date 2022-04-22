using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Monolith2LogicApps3.Application.CsvConversion;
using Monolith2LogicApps3.Domain;

namespace Importer
{
    public class ImportFunction
    {

        private const string LOCATION_NAME = "TypeA";

        [FunctionName("QueueImportFunction")]
        [StorageAccount("AzureWebJobsStorage")]
        [return: ServiceBus("report-type-a", Connection = "ServiceBusConnection")]
        public async Task<string> Run(
            [ServiceBusTrigger("import-type-a", AutoCompleteMessages = true, Connection = "ServiceBusConnection")] string message,
            [Blob("import", FileAccess.ReadWrite, Connection = "Monolith2LogicAppsStorage")] BlobContainerClient blobContainerClient,
            [Sql("dbo.TypeAs", ConnectionStringSetting = "SqlConnection")] IAsyncCollector<TypeA> typeACollector,
            ILogger log)
        {
            log.LogInformation($"C# Service Bus Queue trigger function executed at: {DateTime.Now}, with message: {message}");
            var items = await ReadItems(blobContainerClient, log);
            if (!items.Any())
                return null;
            await ImportTypeAFiles(items, typeACollector);
            return "Send report";
        }

        private async Task<IEnumerable<TypeA>> ReadItems(
            BlobContainerClient blobContainerClient,
            ILogger log)
        {
            var items = new List<TypeA>();
            var reader = new CsvReader();
            var converter = new TypeACsvLineConverter();
            await foreach (var blobItem in blobContainerClient.GetBlobsAsync(BlobTraits.All, BlobStates.None, LOCATION_NAME + "/"))
            {
                try
                {
                    var blobClient = blobContainerClient.GetBlobClient(blobItem.Name);
                    var blobDownload = await blobClient.DownloadContentAsync();
                    var blobStream = blobDownload.Value.Content.ToStream();
                    var importedItems = reader.Parse(blobStream, converter);
                    var modifiedDateTime = DateTime.UtcNow;
                    foreach (var item in importedItems)
                        item.ModifiedDateTime = modifiedDateTime;
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
            await typeACollector.FlushAsync();
        }

    }
}
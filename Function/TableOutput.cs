using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Azure.Data.Tables;
using Azure;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.Http;
using Azure.Storage.Blobs;
using System.Text;


namespace MenuFunctionOutput
{
   public class TableOutput
    {
        public record Order: ITableEntity
        {
            public string PartitionKey { get; set; } = "Orders";
            public string RowKey { get; set; } 
            public decimal Amount { get; set; }
            public string Status { get; set; } = "Processed";
            public DateTimeOffset? Timestamp { get; set; }
            public ETag ETag { get; set; }

        }

        [FunctionName("TableOutput")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
         
            var CONNECTION_STRING = Environment.GetEnvironmentVariable("ConnectionString");
            string GUID = Guid.NewGuid().ToString();

            string BLOB_CONTAINER = "order-blobs";
            byte[] byteArray = Encoding.ASCII.GetBytes(requestBody);
            Stream blob = new MemoryStream(byteArray);
            BlobClient blobClient = new BlobClient(CONNECTION_STRING, BLOB_CONTAINER, $"{GUID}.json");
            blobClient.Upload(blob);

            TableClient tableClient = new TableClient(CONNECTION_STRING , "Orders");
            await tableClient.CreateIfNotExistsAsync();

            var Orderitem = new Order()
            {
                Amount = data.priceCalculated,
                RowKey = GUID
            };

            await tableClient.AddEntityAsync<Order>(Orderitem);

            return new OkObjectResult(data.priceCalculated);

        }
    }
}

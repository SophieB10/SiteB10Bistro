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

        // "{"priceCalculated":"11.00","ordqwerqweer":"Chocolate gateau,Margarita lemon"}"

        public class IncomingOrderMessage
        {
            public decimal priceCalculated { get; set; }
            public string order { get; set; }
        }

        private string CONNECTION_STRING;
        private  BlobContainerClient container;
        const string BLOB_CONTAINER = "orderblobs";

        const string TABLE_NAME = "Orders";



        public TableOutput(){
            CONNECTION_STRING = Environment.GetEnvironmentVariable("ConnectionString");
            container = new BlobContainerClient(CONNECTION_STRING, BLOB_CONTAINER);
        }

        [FunctionName("TableOutput")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {         

            string reqBody = await req.ReadAsStringAsync();      

            IncomingOrderMessage data;

            // Input guard
            try{
                data = JsonConvert.DeserializeObject<IncomingOrderMessage>(reqBody);
                var valid = string.IsNullOrWhiteSpace(data.order);
                if(!valid) return new BadRequestObjectResult("Order is null 😢");
            }
            catch(Exception e){
                var msg = $"Hepl! Could not deserialize msg: {reqBody}";
                log.LogError(e,msg);
                return new BadRequestObjectResult(msg);
            }             
            var GUID = Guid.NewGuid();       

            try{
                await UploadFile(GUID,req.Body);
            }  
            catch(Exception e){
                var msg = $"Hepl! Error uploading file: {reqBody}";
                log.LogError(e,msg);
                return new BadRequestObjectResult(msg);
            }       

            try{
               await InsertTableRecord(GUID,data);
            }  
            catch(Exception e){
                var msg = $"Hepl! Error inserting record in table: {reqBody}";
                log.LogError(e,msg);
                return new BadRequestObjectResult(msg);
            }  
            

            return new OkObjectResult(data.priceCalculated);
        }

        private async Task UploadFile(Guid guid, Stream stream){
             // Create new blob and upload
            using var reader = new BinaryReader(stream);
            var data = reader.ReadBytes((int)stream.Length);
            if(data.Length == 0) throw new Exception("HEPL could not read bytes off of request stream 😢");
            
            //HIER WETEN WE ZEKER DAT WE BYTES HEBBEN!
            await container.CreateIfNotExistsAsync();
            BlobClient blobClient = new BlobClient(CONNECTION_STRING, BLOB_CONTAINER, $"{guid}.json");
            
            using var newStream = new MemoryStream(data, writable: false);
            await blobClient.UploadAsync(newStream);
        }

        private async Task InsertTableRecord(Guid guid,IncomingOrderMessage data ){
            // Insert Table record
            TableClient tableClient = new TableClient(CONNECTION_STRING , TABLE_NAME);
            await tableClient.CreateIfNotExistsAsync();
            var Orderitem = new Order()
            {
                Amount = data.priceCalculated,
                RowKey = guid.ToString()
            };
            await tableClient.AddEntityAsync<Order>(Orderitem);
        }
    }
}

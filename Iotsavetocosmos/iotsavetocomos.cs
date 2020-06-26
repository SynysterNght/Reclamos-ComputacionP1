using IoTHubTrigger = Microsoft.Azure.WebJobs.EventHubTriggerAttribute;


namespace Iotsavetocosmos
{

    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Host;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Azure.EventHubs;
    using System.Text;
    using System.Net.Http;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using System.Threading.Tasks;
    using System;
    using Microsoft.AspNetCore.Http;


    using Iotsavetocosmos.Helpers;
    using Microsoft.Azure.Documents.Client;

    public static class iotsavetocomos
    {
        private static HttpClient cliente = new HttpClient();

        [FunctionName("SavetoCosmos")]
        public static async Task RunAsync([IoTHubTrigger("messages/events", Connection = "connectionString")]EventData message, ILogger log)
        {
            log.LogInformation($"C# IoT Hub trigger function processed a message: {Encoding.UTF8.GetString(message.Body.Array)}");
            var data = JsonConvert.DeserializeObject<iot>(Encoding.UTF8.GetString(message.Body.Array));
            
            var datos = new iot
            {
                messageId = data.messageId,
                deviceId = data.deviceId,
                temperature = data.temperature,
                humidity = data.humidity
            };
            await SaveData(datos);
            
        }

        private static async Task<IActionResult> SaveData(iot obj)
        {
            IActionResult returnValue = null;
            DocumentClient client;
            client = new DocumentClient(new Uri("https://syncosmos.documents.azure.com:443/"), Constants.COSMOS_DB_PRIMMARY_KEY);
            try
            {
                var collectionUri = UriFactory.CreateDocumentCollectionUri(Constants.COSMOS_DB_DATABASE_NAME, Constants.COSMOS_DB_CONTAINER_NAME);
                var documentResponse = await client.CreateDocumentAsync(collectionUri, obj);
                returnValue = new OkObjectResult(obj);
            }
            catch (Exception ex)
            {
                returnValue = new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

            return returnValue;
        }

    }

    
}
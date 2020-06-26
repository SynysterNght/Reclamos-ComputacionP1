
namespace SaveToCosmossyn
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.Http;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using Models;
    using Helpers;
    using System.Collections.Generic;
    public static class SaveToCosmosIot
    {
        [FunctionName("SaveToCloud")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous,  "post", Route = null)] HttpRequest req,
            [CosmosDB(
                        databaseName: Constants.COSMOS_DB_DATABASE_NAME,
                        collectionName:Constants.COSMOS_DB_CONTAINER_NAME,
                        ConnectionStringSetting = "StrCosmos"
                        )]
            IAsyncCollector<object> datos,
            ILogger log)
        {


            IActionResult returnvalue = null;
            try
            {


                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var data = JsonConvert.DeserializeObject<iot>(requestBody);

                
                var iotdata = new iot
                {
                    messageId = data.messageId,
                    deviceId = data.deviceId,
                    temperature = data.temperature,
                    humidity = data.humidity
                };

                await datos.AddAsync(iotdata);
                log.LogInformation($"IOT DATA created {iotdata.temperature}");



                returnvalue = new OkObjectResult(iotdata);
            }
            catch (Exception ex)
            {
                log.LogError($"Could not Save Data. Exception: {ex.Message}");
                returnvalue = new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
            return returnvalue;

            
        }
    }
}

namespace CreateSupport
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

    public class FCreateSupport

    {
        [FunctionName("Function1")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous,  "post", Route = null)] HttpRequest req,
            [CosmosDB(
                        databaseName: Constants.COSMOS_DB_DATABASE_NAME,
                        collectionName:Constants.COSMOS_DB_CONTAINER_NAME,
                        ConnectionStringSetting = "StrCosmos"
                        )]
            IAsyncCollector<object> supports,
            ILogger log)
        {
            
            IActionResult returnvalue = null;
            try
            {

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var data = JsonConvert.DeserializeObject<Support>(requestBody);
                var support = new Support
                {

                    Id=data.Id,
                    ClientId = data.ClientId,
                    Type = data.Type,
                    Subject = data.Subject,
                    Description = data.Description,
                    Status = "En espera",
                    InitialDate = DateTime.Now.ToString()
                    ///usandose "MM/DD/YYYY HH:MM:SS AM"
                    //"yyyy’-‘MM’-‘dd’  ’HH’:’mm’:’ss"

                };

                await supports.AddAsync(support);
                log.LogInformation($"Support Created {support.Subject}");

                returnvalue = new OkObjectResult(support);
            }
            catch (Exception ex)
            {
                log.LogError($"Could not create support. Exception: {ex.Message}");
                returnvalue = new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
            return returnvalue ;
        }
    }
}

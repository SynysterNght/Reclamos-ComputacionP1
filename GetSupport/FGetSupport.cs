namespace GetSupport
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
    using Helpers;
    using GetSupport.Models;
    using System.Collections;
    using System.Collections.Generic;

    public class FGetSupport
    {
        [FunctionName("FGetSupport")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            [CosmosDB(
                        databaseName: Constants.COSMOS_DB_DATABASE_NAME,
                        collectionName: Constants.COSMOS_DB_CONTAINER_NAME,
                        ConnectionStringSetting = "StrCosmos",
                        SqlQuery ="SELECT * FROM c"

                        )]
            IEnumerable<Support> supports,
            ILogger log)
        {
            if (supports == null)
            {
                return new NotFoundResult();
            }

            //string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            //var data = JsonConvert.DeserializeObject(requestBody);
            

            
            return new OkObjectResult(supports);
        }
    }
}

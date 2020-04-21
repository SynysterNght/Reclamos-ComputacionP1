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
    using System.Linq;

    public class FGetSupport
    {

        [FunctionName("FGetSupport")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "FGetSupport/{id}")] HttpRequest req,
            [CosmosDB(
                        databaseName: Constants.COSMOS_DB_DATABASE_NAME,
                        collectionName: Constants.COSMOS_DB_CONTAINER_NAME,
                        ConnectionStringSetting = "StrCosmos",
                        SqlQuery ="SELECT * FROM c WHERE c.id={id}"

                        )]
            IEnumerable<Support> supports,
            ILogger log,
            string id)
        {
         ///////Se necesita poner en la direccion el id como url no como parametro   

            if (supports == null)
            {
                return new NotFoundResult();
            }

          
            return new OkObjectResult(supports);
        }
    }
}

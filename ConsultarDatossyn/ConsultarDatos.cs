

namespace ConsultarDatossyn
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.Http;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using ConsultarDatossyn.Helpers;
    using ConsultarDatossyn.Models;

    public static class ConsultarDatos
    {
        [FunctionName("ConsultarDatosIot")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            [CosmosDB(
                databaseName: Constants.COSMOS_DB_DATABASE_NAME,
                collectionName: Constants.COSMOS_DB_CONTAINER_NAME,
                ConnectionStringSetting = "StrCosmos",
                SqlQuery ="SELECT top 10 * FROM c order by c._ts desc")] IEnumerable<iot> iotdata,
            ILogger log)
            
        {
            if (iotdata == null)
            {
                return new NotFoundResult();
            }

            return new OkObjectResult(iotdata);
        }
    }
}

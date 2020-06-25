

namespace DeleteSupport
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
    using Microsoft.Azure.Documents.Client;
    using System.Linq;
    using Microsoft.Azure.Documents;
    using DeleteSupport.Helpers;
    using DeleteSupport.Models;

    public static class FDeleteSupport
    {
        [FunctionName("FDeleteSupport")]
        public static async Task<IActionResult> UpdateTaskItem(
           [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "FDeleteSupport/{id}")] HttpRequest req,
           [CosmosDB(
            databaseName: Constants.COSMOS_DB_DATABASE_NAME,
            collectionName: Constants.COSMOS_DB_CONTAINER_NAME,
            ConnectionStringSetting = "StrCosmos"
           )] DocumentClient pago,
           ILogger logger,
           string id)
        {
            IActionResult returnvalue = null;
            try
            {
                    string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var option = new FeedOptions { EnableCrossPartitionQuery = true };

                var updatedTask = JsonConvert.DeserializeObject<Support>(requestBody);

                Uri taskCollectionUri = UriFactory.CreateDocumentCollectionUri(Constants.COSMOS_DB_DATABASE_NAME, Constants.COSMOS_DB_CONTAINER_NAME);

                var document = pago.CreateDocumentQuery(taskCollectionUri, option)
                    .Where(t => t.Id == id)
                    .AsEnumerable()
                    .FirstOrDefault();

                if (document == null)
                {
                    logger.LogError($"TaskItem {id} not found. It may not exist!");
                    returnvalue = new OkObjectResult("No existe el ID ingresado");
                }
                else {

                    //bool pagado = document.GetPropertyValue<bool>("pagado");
                    //pagado = true;


                    await pago.DeleteDocumentAsync(document.SelfLink, new RequestOptions { PartitionKey = new PartitionKey(document.Id) });


                    returnvalue = new OkObjectResult("se Elimino correctamente  ");
                }
              
            }
            catch (Exception ex)
            {
                logger.LogError($"Could not delete support. Exception: {ex.Message}");
                returnvalue = new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
            return returnvalue;
        }
    }

    
}

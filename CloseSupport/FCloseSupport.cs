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
using CloseSupport.Models;
using System.Collections.Generic;
using CloseSupport.Helpers;
using System.Linq;
using System.Diagnostics.SymbolStore;

namespace CloseSupport
{
    public static class FCloseSupport
    {
        [FunctionName("FCloseSupport")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "Close/{id}")] HttpRequest req,
            [CosmosDB(ConnectionStringSetting = "StrCosmos")] DocumentClient client,
            ILogger log,
            string id)
        {

            Support support = new Support();
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            var updatedTask = JsonConvert.DeserializeObject<Support>(requestBody);

            Uri taskCollectionUri = UriFactory.CreateDocumentCollectionUri(Constants.COSMOS_DB_DATABASE_NAME, Constants.COSMOS_DB_CONTAINER_NAME);

            var document = client.CreateDocumentQuery(taskCollectionUri)
                .Where(t => t.Id == id)
                .AsEnumerable()
                .FirstOrDefault();

            if (document == null)
            {
                log.LogError($"TaskItem {id} not found. It may not exist!");
                return new NotFoundResult();
            }

            string motivo = req.Query["motivo"];
            document.SetPropertyValue("conclusiondate", DateTime.Now.ToString());
            document.SetPropertyValue("cause",motivo);
            document.SetPropertyValue("status", "Closed");

            await client.ReplaceDocumentAsync(document);

            Support updatedTaskItemDocument = (dynamic)document;

            return new OkObjectResult(updatedTaskItemDocument);
        }
    }
}

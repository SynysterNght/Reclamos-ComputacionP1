using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;
using System.Reflection.Metadata;
using AnswerSupport.Helpers;
using AnswerSupport.Models;
using System.Linq;
using Microsoft.Extensions.ObjectPool;
using System.Collections.Generic;

namespace AnswerSupport
{
    public class FAnswerSupport
    {
        [FunctionName("FAnswerSupport")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "Answer/{id}")] HttpRequest req,
            [CosmosDB(ConnectionStringSetting = "StrCosmos")] DocumentClient client,
            ILogger log,
            string id)
        {
            IActionResult returnvalue = null;
            try
            {
                Support support = new Support();
                List<string> list = new List<string>();
                string respuesta;
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
                else
                {

                    respuesta = document.GetPropertyValue<string>("status");
                    switch (respuesta)
                    {
                        case "Closed":
                            returnvalue = new StatusCodeResult(StatusCodes.Status406NotAcceptable);
                            return returnvalue;
                        case "Pending":
                            document.SetPropertyValue("status", "Answered");
                            break;
                        case "Answered":
                            document.SetPropertyValue("status", "Pending");
                            break;
                        default:
                            Console.WriteLine("Default case");
                            break;
                    }

                    respuesta = req.Query["respuesta"];
                    support.Answers = document.GetPropertyValue<string[]>("answers");
                    support.AnswersDates = document.GetPropertyValue<string[]>("answersdates");

                    list = support.Answers.ToList<string>();
                    list.Add(respuesta);
                    document.SetPropertyValue("answers", list.ToArray<string>());

                    list = support.AnswersDates.ToList<string>();
                    list.Add(DateTime.Now.ToString());
                    document.SetPropertyValue("answersdates", list.ToArray<string>());

                    await client.ReplaceDocumentAsync(document);

                    Support updatedTaskItemDocument = (dynamic)document;

                    returnvalue = new OkObjectResult(updatedTaskItemDocument);
                }
            }
            catch (Exception ex)
            {
                log.LogError($"Could not respond support. Exception: {ex.Message}");
                returnvalue = new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
            return returnvalue;
        }
    }
    
}

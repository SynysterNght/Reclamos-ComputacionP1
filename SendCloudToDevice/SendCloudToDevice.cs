

namespace SendCloudToDevice
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
    using Microsoft.Azure.Devices;
    using System.Text;
    using System.Collections.Generic;
    public static class SendCloudToDevice
    {
        
        static ServiceClient serviceClient;
        [FunctionName("SendMessage")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous,  "post", Route = null)] HttpRequest req,
            ILogger log)
        {

            IActionResult returnvalue = null;
            try
            {

                
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var data = JsonConvert.DeserializeObject<mensajesyn>(requestBody);
                
                var mensaje = new mensajesyn  { mensajeiot=data.mensajeiot };
                

                Console.WriteLine("Send Cloud-to-Device message\n");

                /////////////Mejorar el connectionString
                string cn = Environment.GetEnvironmentVariable("StrIot");
                serviceClient = ServiceClient.CreateFromConnectionString(cn);
                //////////////////

                SendCloudToDeviceMessageAsync(mensaje.mensajeiot).Wait();
              
                returnvalue = new OkObjectResult(mensaje);
            }
            catch (Exception ex)
            {
                log.LogError($"Could not Send Message. Exception: {ex.Message}");
                returnvalue = new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
            return returnvalue;
        }


        private async static Task SendCloudToDeviceMessageAsync(string mensaje)
        {
            var commandMessage = new
             Message(Encoding.ASCII.GetBytes(mensaje));
             await serviceClient.SendAsync("SensonHumedad", commandMessage);
        }
    }
}

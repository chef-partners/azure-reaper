
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using Microsoft.Azure.Documents.Client;
using System.Net;
using Newtonsoft.Json.Linq;

namespace Azure.Reaper
{
    public static class ops
    {
        [FunctionName("ops")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(
                AuthorizationLevel.Function,
                "get",
                "post",
                Route = "{optype}/{id?}"
            )]HttpRequest req,
            string optype,
            string id,
            [CosmosDB(
                ConnectionStringSetting = "azreaper_DOCUMENTDB",
                CreateIfNotExists = true
            )] DocumentClient client,
            ILogger log)
        {

            // Intialise variables
            HttpResponseMessage response = null;
            ResponseMessage msg = new ResponseMessage();
            IEntity entity = null;

            // Instantiate the necessary class based on the 'optype'
            switch (optype)
            {

                case "setting":
                    entity = new Setting(client, log);
                    break;

                default:
                    msg = new ResponseMessage(String.Format("Specified optype is not recognised: {0}", optype), true, HttpStatusCode.NotFound);
                    return msg.CreateResponse();
            }

            // As this is a REST API use the HTTP Method to determine what is required
            if (req.Method == "GET")
            {
                // Set the identifier on the entity so that a search can be performed
                dynamic result = entity.Get(id);

                // Build up the response to return
                msg = entity.GetResponse();
                response = msg.CreateResponse(result);
            }

            return response;
        }

        private static bool IsValidJson(string payload)
        {
            var trimmed = payload.Trim();
            bool result = true;

            if ((trimmed.StartsWith("{") && trimmed.EndsWith("}")) ||
                (trimmed.StartsWith("[") && trimmed.EndsWith("]")))
            {
                try
                {
                    var obj = JToken.Parse(trimmed);
                }
                catch (JsonReaderException)
                {
                    result = false;
                }
            }
            else
            {
                result = false;
            }

            return result;
        }
    }
}

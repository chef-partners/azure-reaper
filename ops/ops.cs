
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

                case "location":
                    entity = new LocationTZ(client, log);
                    break;

                case "subscription":
                    entity = new Subscription(client, log);
                    break;

                case "reaper":
                    Reaper reaper = new Reaper();
                    bool status = await reaper.Process(client, log);
                    msg = new ResponseMessage("Reaper executed", false, HttpStatusCode.OK);
                    return msg.CreateResponse();

                default:
                    msg = new ResponseMessage(String.Format("Specified optype is not recognised: {0}", optype), true, HttpStatusCode.NotFound);
                    return msg.CreateResponse();
            }

            // As this is a REST API use the HTTP Method to determine what is required
            if (req.Method == "GET")
            {
                // Set the identifier on the entity so that a search can be performed
                IEntity result = entity.Get(id);

                // Build up the response to return
                msg = entity.GetResponse();
                response = msg.CreateResponse(result);
            } else {

                // Get the JSON string from the body
                string json = await new StreamReader(req.Body).ReadToEndAsync();

                // Peform checks on the paylod to ensure that it is not not null and is valid JSON
                if (String.IsNullOrWhiteSpace(json))
                {
                    msg.SetError("Body of request must not be empty", true, HttpStatusCode.BadRequest);
                }
                else if (!IsValidJson(json))
                {
                    msg.SetError("Body of request must contain valid JSON data", true, HttpStatusCode.BadRequest);
                }
                else
                {

                    // Parse the json
                    entity.Parse(json);

                    // Determine if there are any errors in the parse
                    msg = entity.GetResponse();
                    if (!msg.IsError())
                    {
                        bool status = await entity.Insert();

                        msg = entity.GetResponse();
                    }
                }

                response = msg.CreateResponse();

            }

            return response;
        }

        private static bool VaidatePayload(string data)
        {
            // Define method variables
            bool valid = true;

            // determmine if the data is a null string
 

            return valid;
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

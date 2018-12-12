using System;
using System.Collections.Generic;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Authentication;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Azure.Reaper
{
    public static class tagger
    {

        [FunctionName("tagger")]
        public static void Run(
            [QueueTrigger("logalertqueue", Connection = "AzureWebJobsStorage")]JObject alertItem,
            [CosmosDB(
                ConnectionStringSetting = "azreaper_DOCUMENTDB",
                CreateIfNotExists = true
            )] DocumentClient client,
            ILogger log
        )
        {

        }
    }
}

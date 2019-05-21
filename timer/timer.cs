using System;
using System.Threading.Tasks;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace Azure.Reaper
{
    public static class timer
    {
        [FunctionName("timer")]
        public static async void Run(
            [TimerTrigger("0 */10 * * * *")]TimerInfo myTimer,
            [CosmosDB(
                ConnectionStringSetting = "azreaper_DOCUMENTDB",
                CreateIfNotExists = true
            )] DocumentClient client,
            ILogger log)
        {
            bool status;
            
            log.LogInformation("Reaper triggered at {0}", DateTime.Now);

            Reaper reaper = new Reaper();
            status = await reaper.Process(client, log);
        }
    }
}

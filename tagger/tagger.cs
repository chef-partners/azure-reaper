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
            [QueueTrigger(
                "logalertqueue",
                Connection = "AzureWebJobsStorage"
            )] ActivityAlert activityLog,
            [CosmosDB(
                ConnectionStringSetting = "azreaper_DOCUMENTDB",
                CreateIfNotExists = true
            )] DocumentClient client,
            ILogger log
        )
        {

            // Determine if a resource group has been created
            if (activityLog.IsCreated())
            {
                log.LogInformation("Considering Resource Group: {0}", activityLog.GetResourceGroupName());

                // Search for the credentials for the subscriptionId recieved
                Subscription sub = new Subscription(client, log);
                Subscription subscription = (Subscription) sub.Get(activityLog.GetSubscriptionId());

                // If a subscription has been found process
                if (subscription == null)
                {
                    log.LogWarning("Credentials for Subscription cannot be found: {0}", activityLog.GetSubscriptionId());
                }
                else if (!subscription.enabled)
                {
                    log.LogWarning("Subscription has been disabled: {0} ({1})", subscription.name, (string) subscription.subscription_id);
                }
                else
                {
                    // Login to Azure and get an azure object to work with
                    IAzure azure = Utilities.AzureLogin(
                        subscription,
                        AzureEnvironment.AzureGlobalCloud,
                        log
                    );

                    // If the subscription contains a resource group with the specified name, create
                    // a resourcegroup object to work with
                    if (azure.ResourceGroups.Contain(activityLog.GetResourceGroupName()))
                    {
                        // Get the tag settings
                        Setting setting = new Setting(client, log);
                        IEnumerable<Setting> settings = setting.GetAllByCategory(new string[] {"tags"});

                        // Retrieve the resource group object
                        IResourceGroup resourceGroup = azure.ResourceGroups.GetByName(activityLog.GetResourceGroupName());
                        ResourceGroup rg = new ResourceGroup(
                            resourceGroup,
                            log,
                            settings,
                            activityLog
                        );

                        // Add the tags that are required for the reaper to function
                        rg.AddDefaultTags();
                    }
                    else
                    {
                        log.LogWarning("{0}: Cannot find resource group in subscription - {1}", activityLog.GetResourceGroupName(), (string) subscription.subscription_id);
                    }
                }
            }
        }
    }
}

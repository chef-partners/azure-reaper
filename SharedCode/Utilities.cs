using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Authentication;
using Microsoft.Extensions.Logging;

namespace Azure.Reaper
{
  public class Utilities
  {
    public static IAzure AzureLogin (
      Subscription subscription,
      AzureEnvironment azureEnvironment,
      ILogger logger
    )
    {
      logger.LogDebug("Attempting to authenticate for subscription: {0}", (string) subscription.subscription_id);

      // Create service principal
      ServicePrincipalLoginInformation spn = new ServicePrincipalLoginInformation {
        ClientId = subscription.client_id,
        ClientSecret = subscription.client_secret
      };
    
      /// Create the Azure credentials
      AzureCredentials azureCredential = new AzureCredentials(
        spn,
        subscription.tenant_id,
        azureEnvironment
      );

      // Create and return the Azure object
      IAzure azure =  Microsoft.Azure.Management.Fluent.Azure
                      .Configure()
                      .Authenticate(azureCredential)
                      .WithSubscription(subscription.subscription_id);

      return azure;
    }
  }
}
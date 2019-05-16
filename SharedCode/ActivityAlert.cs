using Newtonsoft.Json;
using System;

namespace Azure.Reaper
{

  public class ActivityAlert
  {

    public string schemaId;
    public ActivityAlertData data;
    public ActivityAlertClaims claims;

    public ActivityAlert()
    {

    }

    /// <summary>
    /// State if the current item has been created or not
    /// This is done by testing to see if subStatus is equal to "created"
    /// </summary>
    /// <returns>bool</returns>
    public bool IsCreated()
    {
      bool result = false;

      if (data.context.activityLog.subStatus != null && data.context.activityLog.subStatus.ToLower() == "created")
      {
        result = true;
      }

      return result;
    }

    public string GetValue(string name)
    {
      string result = String.Empty;

      // if the claims object is null, populate it
      if (claims == null)
      {
        claims = JsonConvert.DeserializeObject<ActivityAlertClaims>(data.context.activityLog.claims);
      }

      switch (name)
      {
        case "tag_owner":
          result = claims.name;
          break;
        case "tag_owner_email":
          result = data.context.activityLog.caller;
          break;
        case "tag_date":
          result = data.context.activityLog.eventTimestamp.ToString("yyyy-MM-ddTHH:mm:ssZ", System.Globalization.CultureInfo.InvariantCulture);
          break;
      }

      return result;
    }

    public string GetResourceGroupName()
    {
      return data.context.activityLog.resourceGroupName;
    }

    public string GetSubscriptionId()
    {
      return data.context.activityLog.subscriptionId;
    }
  }
}
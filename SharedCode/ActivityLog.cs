using System;

namespace Azure.Reaper
{

  public class ActivityLog
  {

    public string subStatus;
    public DateTime eventTimestamp;
    public string resourceGroupName;
    public string subscriptionId;
    public string caller;
    public ActivityLogClaim claim;

    public ActivityLog()
    {

    }

    /// <summary>
    /// State if the current item has been craeted or not
    /// This is done by testing to see if subStatus is equal to "created"
    /// </summary>
    /// <returns>bool</returns>
    public bool IsCreated()
    {
      bool result = false;

      if (subStatus.ToLower() == "created")
      {
        result = true;
      }

      return result;
    }

    public string GetValue(string name)
    {
      string result = String.Empty;
      switch (name)
      {
        case "tag_owner":
          result = claim.ownerName;
          break;
        case "tag_owner_email":
          result = caller;
          break;
        case "tag_date":
          result = eventTimestamp.ToString("yyyy-MM-ddTHH:mm:ssZ", System.Globalization.CultureInfo.InvariantCulture);
          break;
      }

      return result;
    }
  }
}
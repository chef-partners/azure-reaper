using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;

namespace Azure.Reaper
{

  /// <summary>
  /// 
  /// </summary>
  /// <typeparam name="NotificationDelay"></typeparam>
  [JsonObject]
  public class NotificationDelay : Entity<NotificationDelay>, IEntity
  {
    // Define public properties of the class
    // These are the fields that will be added to the document in the collection
    public string group_name { get; set; }
    public string name { get; set; }
    public string type { get; set; }
    public string subscription { get; set; }
    public DateTime last_notified { get; set; }

    public NotificationDelay(DocumentClient client, ILogger log)
    {
      // set the client and the name of the collection for documents of this type
      this.client = client;
      this.collectionName = "notificationDelay";
      this.criteriaFields = new string[] { "subscription_id", "name", "id" };
      this.logger = log;
    }
  
  }
}
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Azure.Reaper
{

  /// <summary>
  /// 
  /// </summary>
  /// <typeparam name="Subscription"></typeparam>
  [JsonObject]
  public class Subscription : Entity<Subscription>, IEntity
  {
    // Define public properties of the class
    // These are the fields that will be added to the document in the collection
    public string name { get; set; }
    public dynamic subscription_id { get; set; }
    public string client_id { get; set; }
    public string client_secret { get; set; }
    public string tenant_id { get; set; }
    public bool enabled { get; set; }
    public bool reaper { get; set; } = true;

    public Subscription(DocumentClient client, ILogger log)
    {
      // set the client and the name of the collection for documents of this type
      this.client = client;
      this.collectionName = "subscriptions";
      this.criteriaFields = new string[] { "subscription_id", "name", "id" };
      this.logger = log;
    }
  
  }
}
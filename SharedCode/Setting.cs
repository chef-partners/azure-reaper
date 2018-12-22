using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Linq;

namespace Azure.Reaper
{

  /// <summary>
  /// 
  /// </summary>
  /// <typeparam name="Setting"></typeparam>
  [JsonObject]
  public class Setting : Entity<Setting>, IEntity
  {
    // Define public properties of the class
    // These are the fields that will be added to the document in the collection
    public string name { get; set; }
    public dynamic value { get; set; }
    public string category { get; set; }
    [JsonProperty("display_name")]
    public string displayName { get; set; }
    public string description { get; set; }

    public Setting(DocumentClient client, ILogger log)
    {
      // set the client and the name of the collection for documents of this type
      this.client = client;
      this.collectionName = "settings";
      this.criteriaFields = new string[] { "name", "id" };
      this.logger = log;
    }
  
    public dynamic GetItem(string name)
    {
      return items.First(i => i.name == name).value;
    }
  }
}
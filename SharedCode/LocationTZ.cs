using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Azure.Reaper
{
    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="LocationTZ"></typeparam>
    [JsonObject]
    public class LocationTZ : Entity<LocationTZ>, IEntity
    {
        // Define public properties of the class
        // These are the fields that will be added to the document in the collection
        public string name { get; set; }
        public string tzId { get; set; }

        public LocationTZ(DocumentClient client, ILogger log)
        {
            this.client = client;
            this.collectionName = "locationTimezones";
            this.criteriaFields = new string [] { "name", "tzId", "id" };
            this.logger = log;
        }
    }
}
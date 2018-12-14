using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;


namespace Azure.Reaper {

  public class Entity<T>
  {
    protected List<T> items;
    protected string collectionName;
    protected DocumentClient client;
    private string databaseId = "reaper";
    protected Uri collectionLink;
    protected Uri databaseLink;
    protected string[] criteriaFields;
    protected ILogger logger;
    protected ResponseMessage response = new ResponseMessage();

    public string id;

    public IEntity Get(dynamic identifier, bool first = true)
    {
      IEnumerable<IEntity> result = null;
      IEntity doc = null;

      // Set the collection link to perform the search
      SetCollectionLink();

      // Get the SQL statement to execute
      string sqlStatement = BuildSQLStatement(identifier);

      // Execute the query
      // Get the name of the class so that the query can deseralize into the correct objects
      if (this is Setting)
      {
          result = client.CreateDocumentQuery<Setting>(collectionLink, sqlStatement)
                      .AsEnumerable();
      }

      // If no items have been found set the response
      if (Enumerable.Count(result) == 0)
      {
        response.SetError("Unable to find item", true, HttpStatusCode.NotFound);
      }
      else if (Enumerable.Count(result) == 1 && first)
      {
        // As only one document has been found, return that
        doc = Enumerable.First(result);
      }

      return doc;
    }

    public ResponseMessage GetResponse()
    {
      return response;
    }

    public void Parse(string json)
    {
      // Attempt to deserialise the json into a List of objects
      items = JsonConvert.DeserializeObject<List<T>>(json);

      // determine if any items have been set
      if (items.Count == 0)
      {
        response.SetError("An array of items is exepected", true, HttpStatusCode.BadRequest);
      }
    }

    private void SetCollectionLink()
    {
      databaseLink = UriFactory.CreateDatabaseUri(databaseId);
      collectionLink = UriFactory.CreateDocumentCollectionUri(databaseId, collectionName);
    }

    private string BuildSQLStatement(dynamic identifier)
    {
      // Initialise an array to hold the criteria for the statement
      ArrayList criteria = new ArrayList();

      // Set the operator that will join the criteria together
      string joinOperator;

      // Iterate around the criteria fields that are set on the class
      if ((identifier.GetType()).IsArray)
      {
        joinOperator = "AND";

        // iterate around the fields and set the corresponding value in the identifier array
        for (int i = 0; i < identifier.Count; i ++)
        {
          if (identifier[i] is bool || identifier[i] is int)
          {
            criteria.Add(String.Format("t.{0} = {1}", criteriaFields[i], identifier[i]));
          }
          else if (identifier[i] is string)
          {
            criteria.Add(String.Format("t.{0} = '{1}'", criteriaFields[i], identifier[i]));
          }          
        }
      }
      else
      {
        joinOperator = "OR";
        foreach (string field in criteriaFields)
        {
          // If the identifier is a single entity, e.g. not an array ensure that
          // all criteria is set to the same value
          if (identifier is bool || identifier is int)
          {
            criteria.Add(String.Format("t.{0} = {1}", field, identifier));
          }
          else if (identifier is string)
          {
            criteria.Add(String.Format("t.{0} = '{1}'", field, identifier));
          }
        }
      }

      // Create the necessary SQL statement
      string whereClause = String.Join(String.Format(" {0} ", joinOperator), criteria.ToArray());
      string sqlStatement = String.Format("SELECT * FROM {0} t WHERE {1}", this.collectionName, whereClause);

      logger.LogDebug(sqlStatement);

      // Return the sql statement to the calling function
      return sqlStatement;
    }

    public IEnumerator<T> GetEnumerator()
    {
      return items.GetEnumerator();
    }

    public async Task<bool> Insert()
    {
      // create variable to determine how many items are in error
      bool status = false;
      int errorItems = 0;

      // Iterate around the items checking the schema
      foreach (var item in items)
      {
        bool valid = IsSchemaValid(item, false);
        if (!valid)
        {
          errorItems ++;
        }
      }

      // if there are error items, set an error to retiurn
      if (errorItems > 0)
      {
        response.SetError(
          String.Format(
            "There are {0} item(s) that do not conform to the schema for this type",
            errorItems
          ),
          true,
          HttpStatusCode.BadRequest
        );
      }
      else
      {
        // Set the collection link for where the document needs to be stored
        SetCollectionLink();

        // Initialise counters
        int updated = 0;
        int created = 0;

        // Ensure that the database and the collection exist
        bool databaseExists = await CreateDatabase();
        bool collectionExists = await CreateDocumentCollection();

        // if the collection exists attempt to add the document
        if (collectionExists)
        {
          // Iterate around the items to determine if any already exist
          foreach (IEntity item in items)
          {
            // call the get method to determine if the item exists or not
            IEntity exists = Get(item.name);

            // if the exists is null add the item
            if (exists == null)
            {
              logger.LogInformation("Item does not exist");
              await client.CreateDocumentAsync(collectionLink, item);

              // update the created count
              created ++;
            }
            else
            {
              logger.LogInformation("Item needs to be updated");
              await client.UpsertDocumentAsync(collectionLink, item);

              // update the updated count
              updated ++;
            }
          }

          // Set the repsonse message and the status code
          response.SetMessage(String.Format("{0} documents created, {1} documents updated", created.ToString(), updated.ToString()));
          response.SetStatusCode(HttpStatusCode.Created);
          response.SetError(false);
        }

      }

      return status;
    }

    private bool IsSchemaValid(dynamic obj, bool update)
    {
      // set the valid flag
      bool valid = false;

      // Get all the public properties of the class
      PropertyInfo[] publicProps = obj.GetType().GetProperties();

      // Define array to state which values are incorrect
      ArrayList notValid = new ArrayList();

      // if this is an update then ensure that the name is set
      if (update)
      {
        if (String.IsNullOrEmpty(obj.name))
        {
          notValid.Add("name");
        }
      }
      else
      {
        // iterate around all of the properties and ensure that the
        // properties have been set as this is a new item
        foreach (var prop in publicProps)
        {
          // get the type of the property so that the relevant check can be performed
          // Ensure that the value is not null
          if (prop.GetValue(obj) == null)
          {
            logger.LogDebug("Property '{0}' is invalid", prop.Name);
            notValid.Add(prop.Name);
          }
        }
      }

      // if there are no items in notValid set the valid to true
      // else set an error
      if (notValid.Count == 0)
      {
        valid = true;
      }

      return valid;
    }

    private async Task<bool> CreateDatabase()
    {
      // Set flag for the status of the operation
      bool status = true;

      // Ensure that the database exists
      try
      {
        logger.LogDebug("Checking database exists: {0}", databaseId);
        await client.ReadDatabaseAsync(databaseLink);
      }
      catch (DocumentClientException exception)
      {
        // if the database cannot be found create it
        if (exception.StatusCode == HttpStatusCode.NotFound)
        {
          logger.LogInformation("Creating database: {0}", databaseId);
          await client.CreateDatabaseAsync(
            new Database { Id = databaseId }
          );
        }
        else
        {
          logger.LogError("Issue reading database: {0}", exception.Message);
          response.SetError(String.Format("There was a problem with the database: {0}", databaseId), true, HttpStatusCode.InternalServerError);
          status = false;
        }
      }

      return status;
    }

    private async Task<bool> CreateDocumentCollection()
    {
      // Set flag for the status of the operation
      bool status = true;

      // Ensure that the database exists
      try
      {
        logger.LogDebug("Checking collection exists: {0}", collectionName);
        await client.ReadDocumentCollectionAsync(collectionLink);
      }
      catch (DocumentClientException exception)
      {
        // if the database cannot be found create it
        if (exception.StatusCode == HttpStatusCode.NotFound)
        {
          logger.LogInformation("Creating collection: {0}", collectionName);
          await client.CreateDocumentCollectionAsync(
            databaseLink,
            new DocumentCollection { Id = collectionName }
          );
        }
        else
        {
          logger.LogError("Issue reading collection: {0}", exception.Message);
          response.SetError(String.Format("There was a problem with the collection: {0}", collectionName), true, HttpStatusCode.InternalServerError);
          status = false;
        }
      }

      return status;
    }

    protected void SetLogger(ILogger logger)
    {
      this.logger = logger;
    }
  }



}
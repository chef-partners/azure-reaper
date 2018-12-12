using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Logging;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;


namespace Azure.Reaper {

  public class Entity<T>
  {
    protected List<T> items;
    protected string collectionName;
    protected DocumentClient client;
    private string databaseId = "reaper";
    protected Uri collectionLink;
    protected string[] criteriaFields;
    protected ILogger logger;
    protected ResponseMessage response;

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

    private void SetCollectionLink()
    {
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
  }

}
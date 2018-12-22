using System.Collections;
using System.Threading.Tasks;

namespace Azure.Reaper {

  public interface IEntity
  {
    string name { get; set; }
    IEntity Get(dynamic identifier, bool first = true);
    IEntity Get(dynamic[] identifier, string[] fields, bool first = true);
    dynamic GetAllByCategory(string[] categories);
    dynamic GetAll();
    dynamic GetUsingSQL(string sqlStatement);
    ResponseMessage GetResponse();
    void Parse(string json);
    Task<bool> Insert();
  }
}
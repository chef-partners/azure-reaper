using System.Threading.Tasks;

namespace Azure.Reaper {

  public interface IEntity
  {
    string name { get; set; }
    IEntity Get(dynamic identifier, bool first = true);
    ResponseMessage GetResponse();
    void Parse(string json);
    Task<bool> Insert();
  }
}
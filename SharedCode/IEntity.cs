namespace Azure.Reaper {

  public interface IEntity
  {
    IEntity Get(dynamic identifier, bool first = true);
    ResponseMessage GetResponse();
  }
}
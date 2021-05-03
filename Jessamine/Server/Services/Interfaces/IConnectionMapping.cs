using System.Collections.Generic;

namespace Jessamine.Server.Services.Interfaces
{
  public interface IConnectionMapping<T>
  {
    void Add(T key, string connectionId);

    IEnumerable<string> GetConnections(T key);

    T GetUser(string connectionId);

    void Remove(T key, string connectionId);
  }
}
using System.Collections.Generic;
using System.Linq;
using Jessamine.Server.Services.Interfaces;

namespace Jessamine.Server.Services
{
  public class ConnectionMapping<T> : IConnectionMapping<T>
  {
    private readonly Dictionary<T, HashSet<string>> _connections =
      new Dictionary<T, HashSet<string>>();

    public int Count
    {
      get
      {
        return _connections.Count;
      }
    }

    public void Add(T key, string connectionId)
    {
      lock (_connections)
      {
        HashSet<string> connections;
        if (!_connections.TryGetValue(key, out connections))
        {
          connections = new HashSet<string>();
          _connections.Add(key, connections);
        }

        lock (connections)
        {
          connections.Add(connectionId);
        }
      }
    }

    public IEnumerable<string> GetConnections(T key)
    {
      HashSet<string> connections;
      if (_connections.TryGetValue(key, out connections))
      {
        return connections;
      }

      return Enumerable.Empty<string>();
    }

    public T GetUser(string connectionId)
    {
      T user = _connections.FirstOrDefault(x => x.Value.Any(y => y == connectionId)).Key;

      return user;
    }

    public void Remove(T key, string connectionId)
    {
      lock (_connections)
      {
        HashSet<string> connections;
        if (!_connections.TryGetValue(key, out connections))
        {
          return;
        }

        lock (connections)
        {
          connections.Remove(connectionId);

          if (connections.Count == 0)
          {
            _connections.Remove(key);
          }
        }
      }
    }
  }
}

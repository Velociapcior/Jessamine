using System.IO.Enumeration;
using System.Text;
using Jessamine.Server.Models;
using Microsoft.Extensions.Primitives;

namespace Jessamine.Server.Services.Interfaces
{
  public interface IPairingProvider
  {
    bool PairUser(string connectionId);

    string FindPair(string connectionId);

    Pair RemovePair(string connectionId);
  }
}
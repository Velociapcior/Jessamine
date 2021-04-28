using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jessamine.Server.Models;
using Jessamine.Server.Services.Interfaces;

namespace Jessamine.Server.Services
{
  public class PairingProvider : IPairingProvider
  {
    private readonly HashSet<Pair> _pairedUsers;

    public PairingProvider()
    {
      _pairedUsers = new HashSet<Pair>();
    }

    public void PairUser(string connectionId)
    {
      var pair = _pairedUsers.FirstOrDefault(x => string.IsNullOrEmpty(x.SecondUser));

      if (pair == null)
      {
        _pairedUsers.Add(new Pair(connectionId));
      }
      else
      {
        pair.SecondUser = connectionId;
      }
    }
  }
}

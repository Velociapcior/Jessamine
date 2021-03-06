using System;
using System.Collections.Concurrent;
using System.Linq;
using Jessamine.Server.Data.Models;
using Jessamine.Server.Services.Interfaces;

namespace Jessamine.Server.Services
{
  public class PairingProvider : IPairingProvider
  {
    private readonly ConcurrentBag<Pair> _pairedUsers;

    public PairingProvider()
    {
      _pairedUsers = new ConcurrentBag<Pair>();
    }

    public bool PairUser(string connectionId)
    {
      var pair = _pairedUsers.FirstOrDefault(x => string.IsNullOrEmpty(x.SecondUser) && x.FirstUser != connectionId);

      if (pair == null)
      {
        _pairedUsers.Add(new Pair(connectionId));

        return false;
      }
      else
      {
        pair.SecondUser = connectionId;

        return true;
      }
    }

    public string FindPairedUser(string connectionId)
    {
      var pair = _pairedUsers.First(x => x.FirstUser == connectionId || x.SecondUser == connectionId);

      if (pair.FirstUser == connectionId)
      {
        return pair.SecondUser;
      }

      return pair.FirstUser;
    }

    public Pair FindPair(string connectionId)
    {
      var pair = _pairedUsers.First(x => x.FirstUser == connectionId || x.SecondUser == connectionId);
      
      return pair;
    }

    public Pair GetPair(string firstParticipant, string secondParticipant)
    {
      Pair pair = _pairedUsers.Single(x => 
        x.FirstUser == firstParticipant && x.SecondUser == secondParticipant ||
        x.FirstUser == secondParticipant && x.SecondUser == firstParticipant);

      return pair;
    }

    public Pair GetPair(long conversationId)
    {
      Pair pair = _pairedUsers.Single(x => x.ConversationId == conversationId);

      return pair;
    }

    public void SetConversation(
      string firstParticipant, 
      string secondParticipant, 
      long conversationId,
      DateTime entityStartedDate)
    {
      Pair pair = GetPair(firstParticipant, secondParticipant);

      pair.ConversationId = conversationId;
      pair.ConversationStartDate = entityStartedDate;
    }

    public Pair RemovePair(string connectionId)
    {
      var pair = _pairedUsers.FirstOrDefault(x => x.FirstUser == connectionId || x.SecondUser == connectionId);
      return RemovePair(pair);
    }

    public Pair RemovePair(long conversationId)
    {
      var pair = _pairedUsers.FirstOrDefault(x => x.ConversationId == conversationId);
      return RemovePair(pair);
    }

    private Pair RemovePair(Pair pair)
    {
      if (pair != null)
      {
        bool isRemoved = _pairedUsers.TryTake(out pair);

        if (isRemoved)
        {
          return pair;
        }
      }

      return null;
    }
  }
}

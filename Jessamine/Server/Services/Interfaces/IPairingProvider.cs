using System;
using Jessamine.Server.Data.Models;

namespace Jessamine.Server.Services.Interfaces
{
  public interface IPairingProvider
  {
    bool PairUser(string connectionId);

    string FindPairedUser(string connectionId);

    Pair FindPair(string connectionId);

    Pair GetPair(string firstParticipant, string secondParticipant);

    Pair GetPair(long conversationId);

    void SetConversation(string firstParticipant, string secondParticipant, long conversationId,
      DateTime startedDate);

    Pair RemovePair(string connectionId);

    Pair RemovePair(long conversationId);
  }
}
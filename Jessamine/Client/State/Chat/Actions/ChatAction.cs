using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jessamine.Client.State.Chat.Actions
{
  public record StartConversation
  {
    public string ConnectionId { get; init; }
    public bool IsConnected { get; init; }
    public long ConversationId { get; init; }
    public string ConnectedUserName { get; init; }

    public StartConversation(string connectionId, bool isConnected, long conversationId, string connectedUserName)
    {
      ConnectionId = connectionId;
      IsConnected = isConnected;
      ConversationId = conversationId;
      ConnectedUserName = connectedUserName;
    }
  }

  public record SetTimer
  {
    public long Ticks { get; init; }

    public SetTimer(long ticks)
    {
      Ticks = ticks;
    }
  }

  public record EndConversation;

  public record UserAgreedToContinue;

  public record ParticipantAgreedToContinue;
}

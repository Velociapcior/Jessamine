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
    public long ConversationId { get; set; }

    public StartConversation(string connectionId, bool isConnected, long conversationId)
    {
      ConnectionId = connectionId;
      IsConnected = isConnected;
      ConversationId = conversationId;
    }
  }

  public record EndConversation
  {
  }
}

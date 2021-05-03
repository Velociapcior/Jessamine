using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fluxor;
using Jessamine.Client.State.Chat.Actions;

namespace Jessamine.Client.State.Chat
{
  public static class ChatReducers
  {
    [ReducerMethod]
    public static ChatState OnStartConversation(ChatState state, StartConversation action) => state with
    {
      IsConnected = action.IsConnected,
      ConnectedUserId = action.ConnectionId
    };

    [ReducerMethod]
    public static ChatState OnEndConversation(ChatState state, EndConversation action) => state with
    {
      IsConnected = false,
      ConnectedUserId = string.Empty,
      ConversationId = -1
    };
  }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fluxor;
using Jessamine.Client.State.Conversation.Actions;

namespace Jessamine.Client.State.Conversation
{
  public static class ConversationReducers
  {
    [ReducerMethod]
    public static ConversationState OnSaveConversations(ConversationState state, SaveConversations action) => state with
    {
      Conversations = action.Conversations
    };
  }
}

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

    [ReducerMethod]
    public static ConversationState OnSetSelectedConversation(
      ConversationState state,
      SetSelectedConversation action) => 
      state with
    {
      SelectedConversationId = action.ConversationId,
      SelectedConversation = state.Conversations.FirstOrDefault(x => x.Id == action.ConversationId)
    };
  }
}

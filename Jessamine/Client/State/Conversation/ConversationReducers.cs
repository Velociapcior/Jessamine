using System.Linq;
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

    [ReducerMethod]
    public static ConversationState OnSetLastMessageId(ConversationState state, SetLastMessageId action) =>
      state with { LastMessageId = action.Id };

    [ReducerMethod]
    public static ConversationState OnSetLastMessageStatus(ConversationState state, SetLastMessageStatus action)
    {
      Jessamine.Shared.Conversation selectedConversation = state.SelectedConversation;

      if (state.SelectedConversation.Id == action.ConversationId)
      {
        selectedConversation.LastMessageStatus = action.Status;
      }

      return state with
      {
        SelectedConversation = selectedConversation
      };
    }
  }
}

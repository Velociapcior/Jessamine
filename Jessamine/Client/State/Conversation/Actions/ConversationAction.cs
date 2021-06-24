using System.Collections.Generic;
using Jessamine.Shared.Common;

namespace Jessamine.Client.State.Conversation.Actions
{
  public record FetchConversations(long? ConversationId);

  public record SaveConversations
  {
    public List<Jessamine.Shared.Conversation> Conversations { get; init; }

    public SaveConversations(List<Jessamine.Shared.Conversation> conversations)
    {
      Conversations = conversations;
    }
  }

  public record SetSelectedConversation(long ConversationId);

  public record SendMessage(Jessamine.Shared.Message Message);

  public record GetNewMessages(long ConversationId, long LastMessageId);

  public record SetLastMessageId(long Id);

  public record UpdateLastMessageStatus(long ConversationId, MessageStatus Status);

  public record SetLastMessageStatus(long ConversationId, MessageStatus Status);
}

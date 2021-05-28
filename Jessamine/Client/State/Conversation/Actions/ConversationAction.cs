using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jessamine.Client.State.Conversation.Actions
{
  public record FetchConversations;

  public record SaveConversations
  {
    public List<Jessamine.Shared.Conversation> Conversations { get; init; }

    public SaveConversations(List<Jessamine.Shared.Conversation> conversations)
    {
      Conversations = conversations;
    }
  }
}

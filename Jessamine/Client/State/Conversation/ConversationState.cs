using System.Collections.Generic;
using Fluxor;

namespace Jessamine.Client.State.Conversation
{
  public record ConversationState
  {
    public List<Jessamine.Shared.Conversation> Conversations { get; init; }

    public long SelectedConversationId { get; set; }
  }

  public class ConversationFeatureState : Feature<ConversationState>
  {
    public override string GetName() => nameof(ConversationState);

    protected override ConversationState GetInitialState() => new()
    {
      Conversations = new List<Jessamine.Shared.Conversation>(),
      SelectedConversationId = default
    };
  }
}

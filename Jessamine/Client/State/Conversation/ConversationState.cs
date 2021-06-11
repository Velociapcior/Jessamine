using System.Collections.Generic;
using Fluxor;
using ConversationModel = Jessamine.Shared.Conversation;

namespace Jessamine.Client.State.Conversation
{
  public record ConversationState
  {
    public List<ConversationModel> Conversations { get; init; }

    public long SelectedConversationId { get; init; }

    public ConversationModel SelectedConversation { get; init; }
  }

  public class ConversationFeatureState : Feature<ConversationState>
  {
    public override string GetName() => nameof(ConversationState);

    protected override ConversationState GetInitialState() => new()
    {
      Conversations = new List<ConversationModel>(),
      SelectedConversationId = default,
      SelectedConversation = new ConversationModel()
    };
  }
}

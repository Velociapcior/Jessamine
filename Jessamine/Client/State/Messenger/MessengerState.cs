using System.Collections.Generic;
using Jessamine.Shared;
using Fluxor;

namespace Jessamine.Client.State.Messenger
{
  public record MessengerState
  {
    public List<Message> Messages { get; init; }

    public string Input { get; init; }
  }

  public class MessengerFeatureState : Feature<MessengerState>
  {
    public override string GetName() => nameof(MessengerState);

    protected override MessengerState GetInitialState() => new()
    {
      Messages = new List<Message>(),
      Input = string.Empty
    };
  }
}

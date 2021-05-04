using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jessamine.Shared;
using Microsoft.AspNetCore.Connections.Features;
using Fluxor;

namespace Jessamine.Client.State.Messenger
{
  public record MessengerState
  {
    public List<Message> Messages { get; init; }
  }

  public class MessengerFeatureState : Feature<MessengerState>
  {
    public override string GetName() => nameof(MessengerState);

    protected override MessengerState GetInitialState() => new()
    {
      Messages = new List<Message>()
    };
  }
}

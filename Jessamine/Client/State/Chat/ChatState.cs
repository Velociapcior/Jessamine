using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Fluxor;

namespace Jessamine.Client.State.Chat
{
  public record ChatState
  {
    public string ConnectedUserId { get; init; }
    public bool IsConnected { get; init; }
    public long ConversationId { get; init; }
    public TimeSpan TimeToEnd { get; init; }
    public bool UserContinue { get; init; }
    public bool ParticipantContinue { get; init; }
  }

  public class ChatFeatureState : Feature<ChatState>
  {
    public override string GetName() => nameof(ChatState);

    protected override ChatState GetInitialState() => new()
    {
      ConnectedUserId = string.Empty,
      IsConnected = false,
      ConversationId = -1,
      TimeToEnd = TimeSpan.MinValue,
      ParticipantContinue = false,
      UserContinue = false
    };
  }
}

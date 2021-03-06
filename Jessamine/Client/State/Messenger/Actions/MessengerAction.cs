using System.Collections.Generic;
using Jessamine.Shared;

namespace Jessamine.Client.State.Messenger.Actions
{
  public record ReceiveMessage
  {
    public Message Message { get; set; }

    public ReceiveMessage(Message message)
    {
      Message = message;
    }
  }

  public record ClearMessenger;

  public record ChangeInput
  {
    public string Input { get; set; }

    public ChangeInput(string input)
    {
      Input = input;
    }
  }

  public record SetMessages(List<Message> Messages);

  public record AppendMessages(List<Message> Messages);
}

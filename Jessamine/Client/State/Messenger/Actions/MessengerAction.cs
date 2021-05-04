using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
}

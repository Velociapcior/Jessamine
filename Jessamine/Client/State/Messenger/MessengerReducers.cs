using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Fluxor;
using Jessamine.Client.State.Messenger.Actions;
using Jessamine.Shared;

namespace Jessamine.Client.State.Messenger
{
  public class MessengerReducers
  {
    [ReducerMethod]
    public static MessengerState OnReceiveMessage(MessengerState state, ReceiveMessage action)
    {
      var messages = state.Messages;

      messages.Add(action.Message);

      return state with
      {
        Messages = messages
      };
    }

    [ReducerMethod]
    public static MessengerState OnInputChanged(MessengerState state, ChangeInput action)
    {
      return state with {Input = action.Input};
    }

    [ReducerMethod]
    public static MessengerState OnState(MessengerState state, ClearMessenger action)
    {
      return state with {Messages = new List<Message>()};
    }
  }
}

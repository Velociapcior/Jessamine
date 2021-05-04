using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Fluxor;
using Jessamine.Client.State.Messenger.Actions;

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
  }
}

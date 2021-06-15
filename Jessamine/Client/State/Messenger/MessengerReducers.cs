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
    public static MessengerState OnInputChanged(MessengerState state, ChangeInput action) =>
      state with {Input = action.Input};

    [ReducerMethod]
    public static MessengerState OnStateClear(MessengerState state, ClearMessenger action) =>
      state with {Messages = new List<Message>()};

    [ReducerMethod]
    public static MessengerState OnSetMessages(MessengerState state, SetMessages action) =>
      state with {Messages = action.Messages};

    [ReducerMethod]
    public static MessengerState OnAppendMessages(MessengerState state, AppendMessages action)
    {
      var messages = state.Messages.Concat(action.Messages);

      return state with { Messages = messages.ToList() };
    }
  }
}
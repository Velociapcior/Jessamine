using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Fluxor;
using Jessamine.Client.State.Conversation.Actions;
using Jessamine.Client.State.Messenger.Actions;
using Jessamine.Shared.Common;

namespace Jessamine.Client.State.Messenger.Effects
{
  public class MessengerEffects
  {
    private readonly HttpClient Http;

    public MessengerEffects(HttpClient http)
    {
      Http = http;
    }

    [EffectMethod]
    public async Task LoadConversation(SetSelectedConversation action, IDispatcher dispatcher)
    {
      List<Jessamine.Shared.Message> messages = await Http.GetFromJsonAsync<List<Jessamine.Shared.Message>>($"api/messages?conversationId={action.ConversationId}");

      dispatcher.Dispatch(new SetMessages(messages));
      
      if (messages is {Count: > 0})
      {
        dispatcher.Dispatch(new SetLastMessageId(messages.Last().Id));

        dispatcher.Dispatch(new UpdateLastMessageStatus(action.ConversationId, MessageStatus.Read));
      }
    }
  }
}

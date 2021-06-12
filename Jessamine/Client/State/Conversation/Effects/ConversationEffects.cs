using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Fluxor;
using Jessamine.Client.Pages;
using Jessamine.Client.State.Conversation.Actions;
using Jessamine.Client.State.Messenger.Actions;

namespace Jessamine.Client.State.Conversation.Effects
{
  public class ConversationEffects
  {
    private readonly HttpClient Http;

    public ConversationEffects(HttpClient http)
    {
      Http = http;
    }

    [EffectMethod]
    public async Task LoadConversations(FetchConversations action, IDispatcher dispatcher)
    {
      List<Jessamine.Shared.Conversation> conversations = await Http.GetFromJsonAsync<List<Jessamine.Shared.Conversation>>("api/conversations");
      
      dispatcher.Dispatch(new SaveConversations(conversations));

      if (conversations is {Count: > 0})
      {
        long selectedConversationId = action.ConversationId ?? conversations.First().Id;

        dispatcher.Dispatch(new SetSelectedConversation(selectedConversationId));
      }
    }

    [EffectMethod]
    public async Task SendMessage(SendMessage action, IDispatcher dispatcher)
    {
      var response = await Http.PostAsJsonAsync("api/messages", action.Message);

      if (response.IsSuccessStatusCode)
      {
        dispatcher.Dispatch(new ReceiveMessage(action.Message));

        dispatcher.Dispatch(new ChangeInput(String.Empty));
      }
    }
  }
}

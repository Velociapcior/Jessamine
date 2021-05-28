using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Fluxor;
using Jessamine.Client.Pages;
using Jessamine.Client.State.Conversation.Actions;

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
    public async Task LoadConversations(FetchConversations actionConversations, IDispatcher dispatcher)
    {
      var conversations = await Http.GetFromJsonAsync<List<Jessamine.Shared.Conversation>>("api/conversations");

      dispatcher.Dispatch(new SaveConversations(conversations));
    }
  }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Fluxor;
using Jessamine.Client.State.Conversation.Actions;
using Jessamine.Client.State.Messenger.Actions;
using Jessamine.Shared.Common;

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

        if (Int32.TryParse(await response.Content.ReadAsStringAsync(), out var messageId))
        {
          dispatcher.Dispatch(new SetLastMessageId(messageId));
        };

        dispatcher.Dispatch(new SetLastMessageStatus(action.Message.ConversationId, MessageStatus.Sent));
      }
    }

    [EffectMethod]
    public async Task GetNewMessages(GetNewMessages action, IDispatcher dispatcher)
    {
      try
      {
        var response = await Http.GetFromJsonAsync <List<Jessamine.Shared.Message>>(
          $"api/messages/new?mesageId={action.LastMessageId}&conversationId={action.ConversationId}");

        if (response is {Count: > 0})
        {
          dispatcher.Dispatch(new SetLastMessageId(response.Last().Id));

          dispatcher.Dispatch(new AppendMessages(response));

          dispatcher.Dispatch(new UpdateLastMessageStatus(action.ConversationId, MessageStatus.Read));
        }
      }
      catch
      {
        Console.WriteLine("could not download new messages, repeating in 3 seconds");
      }
    }

    [EffectMethod]
    public async Task UpdateLastMessageStatus(UpdateLastMessageStatus action, IDispatcher dispatcher)
    {
      var conversation = new Jessamine.Shared.Conversation()
      {
        LastMessageStatus = action.Status
      };

      var response = await Http.PatchAsync($"api/conversations/{action.ConversationId}", JsonContent.Create(conversation));

      if (response.IsSuccessStatusCode)
      {
        dispatcher.Dispatch(new SetLastMessageStatus(action.ConversationId, action.Status));
      }
    }
  }
}

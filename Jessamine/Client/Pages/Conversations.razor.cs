using System;
using System.Linq;
using System.Threading.Tasks;
using Jessamine.Client.State.Conversation.Actions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace Jessamine.Client.Pages
{
  public partial class Conversations
  {
    [Parameter]
    public long? ConversationId { get; set; }
    private string _userName;
    private long SelectedConversationId => _conversationState.Value.SelectedConversationId;

    protected override async Task OnInitializedAsync()
    {
      var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
      var user = authState.User;

      _userName = user.Identity.Name;

      await base.OnInitializedAsync();
    }

    protected override void OnAfterRender(bool firstRender)
    {
      if (firstRender)
      {
        _dispatcher.Dispatch(new FetchConversations(ConversationId));
      }
    }

    private void Send(string input)
    {
      if (string.IsNullOrEmpty(input))
      {
        return;
      }

      var message = new Jessamine.Shared.Message
      {
        Content = input,
        ConversationId = SelectedConversationId,
        Date = DateTime.Now,
        From = _userName,
        To = _conversationState.Value.SelectedConversation.ParticipantName
      };

      _dispatcher.Dispatch(new SendMessage(message));
    }

    private void SelectConversation(long id)
    {
      _dispatcher.Dispatch(new SetSelectedConversation(id));
    }
  }
}

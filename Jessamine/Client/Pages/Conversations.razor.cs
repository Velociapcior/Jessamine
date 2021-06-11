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

    private async Task Send(string input)
    {
    }

    private void SelectConversation(long id)
    {
      _dispatcher.Dispatch(new SetSelectedConversation(id));
    }
  }
}

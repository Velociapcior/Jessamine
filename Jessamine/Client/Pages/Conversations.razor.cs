using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Jessamine.Client.State.Conversation.Actions;
using Jessamine.Shared.Common;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace Jessamine.Client.Pages
{
  public partial class Conversations
  {
    [Parameter]
    public long? ConversationId { get; set; }

    private string _userName;

    private bool _readLastMessage =>
      _conversationState.Value.SelectedConversation.LastMessageStatus == MessageStatus.Read;

    private long SelectedConversationId => _conversationState.Value.SelectedConversationId;

    private long LastMessageId => _conversationState.Value.LastMessageId;

    private Timer _timer;

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

      _timer = new Timer(state =>
      {
        _dispatcher.Dispatch(new GetNewMessages(SelectedConversationId, LastMessageId));
      }, null, 3000, 5000);
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

    public async ValueTask DisposeAsync()
    {
      await new Task(() => {});
    }
  }
}

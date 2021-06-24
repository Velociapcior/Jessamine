using System;
using System.Threading;
using System.Threading.Tasks;
using Jessamine.Client.State.Conversation.Actions;
using Microsoft.AspNetCore.Components;

namespace Jessamine.Client.Pages
{
  public partial class Conversations
  {
    [Parameter]
    public long? ConversationId { get; set; }

    private string _userName;

    private long SelectedConversationId => _conversationState.Value.SelectedConversationId;

    private long LastMessageId => _conversationState.Value.LastMessageId;

    private Timer _timer;
    private readonly ManualResetEvent TimerDisposed;

    public Conversations()
    {
      TimerDisposed = new ManualResetEvent(false);
    }

    protected override async Task OnInitializedAsync()
    {
      var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
      var user = authState.User;

      _userName = user.Identity.Name;

      await base.OnInitializedAsync();
    }

    protected override void OnAfterRender(bool firstRender)
    {
      try
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
      catch (Exception e)
      {
        Console.WriteLine(e);
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

    public new void Dispose()
    {
      _timer.Dispose(TimerDisposed);

      TimerDisposed.WaitOne();
      TimerDisposed.Dispose();

      base.Dispose();
    }
  }
}

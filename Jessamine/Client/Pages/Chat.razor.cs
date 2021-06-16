using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fluxor.Blazor.Web.Components;
using Jessamine.Client.State.Chat.Actions;
using Jessamine.Client.State.Messenger.Actions;
using Jessamine.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace Jessamine.Client.Pages
{
  [Authorize]
  public partial class Chat
  {
    private string _modalDisplay = "none;";
    private string _modalClass = "";
    private bool _showBackdrop = false;

    private HubConnection _hubConnection;

    private Timer _timer;

    private TimeSpan TimeToEnd => _chatState.Value.TimeToEnd;

    private string _userName;

    public bool IsConnected => _chatState.Value.IsConnected;

    public void Open()
    {
      _modalDisplay = "block;";
      _modalClass = "show";
      _showBackdrop = true;
    }

    public void Close()
    {
      _modalDisplay = "none";
      _modalClass = "";
      _showBackdrop = false;
    }

    public async Task OnDeclineClick()
    {
      await _hubConnection.SendAsync("EndConversation", _chatState.Value.ConversationId);
    }

    public async Task OnAgreeClick()
    {
      _dispatcher.Dispatch(new UserAgreedToContinue());

      var conversationId = _chatState.Value.ConversationId;

      await _hubConnection.SendAsync("ParticipantAgreedToContinue", _chatState.Value.ConnectedUserId);

      if (_chatState.Value.UserContinue && _chatState.Value.ParticipantContinue)
      {
        await _hubConnection.SendAsync("AcceptConversation", _chatState.Value.ConversationId);

        _dispatcher.Dispatch(new EndConversation());
        
        GoToConversation(conversationId);
      }
    }

    public void GoToConversation(long conversationId)
    {
      NavigationManager.NavigateTo($"conversations/{conversationId}");
    }

    protected override async Task OnInitializedAsync()
    {
      var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
      var user = authState.User;

      _userName = user.Identity.Name;

      Console.WriteLine($"ChatHub uri: {NavigationManager.BaseUri}, URI: {NavigationManager.Uri}");
    }

    private async Task CreateHubConnection()
    {
      _hubConnection = new HubConnectionBuilder()
        .WithUrl(NavigationManager.ToAbsoluteUri("/hubs/chathub"), options =>
        {
          options.AccessTokenProvider = async () =>
          {
            var accessTokenResult = await AccessTokenProvider.RequestAccessToken();

            accessTokenResult.TryGetToken(out var accessToken);
            return accessToken.Value;
          };
        })
        .Build();

      ConfigureHubConnection();

      await _hubConnection.StartAsync();
    }

    private void ConfigureHubConnection()
    {
      _hubConnection.On<Message>("ReceiveMessage", (message) => { _dispatcher.Dispatch(new ReceiveMessage(message)); });

      _hubConnection.On<bool, string, long, string>("ConnectWithUser",
        (isConnected, connectedUserConnectionId, conversationId, connectedUserName) =>
        {
          _dispatcher.Dispatch(new StartConversation(connectedUserConnectionId, isConnected, conversationId, connectedUserName));

          _timer = new Timer(_ => { _hubConnection.SendAsync("CalculateTimeDifference", DateTime.Now); }, null, 0,
            1000);
        });

      _hubConnection.On("EndConversation", async () =>
      {
        await _timer.DisposeAsync();
        _dispatcher.Dispatch(new EndConversation());
        _dispatcher.Dispatch(new ClearMessenger());

        Close();
        await _hubConnection.SendAsync("QueueForConversation");
      });

      _hubConnection.On<long, bool>("SetTimer", (timeElapsed, timeHasRunOut) =>
      {
        if (timeHasRunOut)
        {
          Open();

          StateHasChanged();

          _timer.DisposeAsync();

          return;
        }

        _dispatcher.Dispatch(new SetTimer(timeElapsed));
      });

      _hubConnection.On("ParticipantAgreedToContinue", async () =>
      {
        _dispatcher.Dispatch(new ParticipantAgreedToContinue());

        long conversationId = _chatState.Value.ConversationId;

        if (_chatState.Value.UserContinue && _chatState.Value.ParticipantContinue)
        {
          await _hubConnection.SendAsync("AcceptConversation", conversationId);

          _dispatcher.Dispatch(new EndConversation());
          GoToConversation(conversationId);
        }
      });
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {   
      if (firstRender)
      {
        try
        {
          await CreateHubConnection();

          await _hubConnection.SendAsync("QueueForConversation");

          _dispatcher.Dispatch(new ClearMessenger());
        }
        catch (NullReferenceException e)
        {
          Console.WriteLine("Error when queue'ing for conversation");
          Console.WriteLine(e.Message);
          throw;
        }

      }
    }

    private async Task Send(string input)
    {
      if (string.IsNullOrEmpty(input))
      {
        return;
      }

      await _hubConnection.SendAsync("SendMessage", _chatState.Value.ConnectedUserId, input,
        _chatState.Value.ConversationId);

      _dispatcher.Dispatch(new ChangeInput(string.Empty));
    }

    public async ValueTask DisposeAsync()
    {
      await _hubConnection.DisposeAsync();

      if (_timer != null)
      {
        await _timer.DisposeAsync();
      }
      
      _dispatcher.Dispatch(new EndConversation());
      _dispatcher.Dispatch(new ClearMessenger());
    }
  }
}

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

    private readonly List<string> _messages = new List<string>();
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

      await _hubConnection.SendAsync("ParticipantAgreedToContinue", _chatState.Value.ConnectedUserId);

      if (_chatState.Value.UserContinue && _chatState.Value.ParticipantContinue)
      {
        _dispatcher.Dispatch(new EndConversation());
        GoToConversation();
      }
    }

    public void GoToConversation()
    {
      NavigationManager.NavigateTo($"conversations/{_chatState.Value.ConversationId}");
    }

    protected override async Task OnInitializedAsync()
    {
      var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
      var user = authState.User;

      _userName = user.Identity.Name;

      Console.WriteLine($"ChatHub uri: {NavigationManager.BaseUri}, URI: {NavigationManager.Uri}");

      await CreateHubConnection();
    }
    private async Task CreateHubConnection()
    {
      _hubConnection = new HubConnectionBuilder()
        .WithUrl(NavigationManager.ToAbsoluteUri("/chathub"), options =>
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

      _hubConnection.On<bool, string, long>("ConnectWithUser",
        (isConnected, connectedUserConnectionId, conversationId) =>
        {
          _dispatcher.Dispatch(new StartConversation(connectedUserConnectionId, isConnected, conversationId));

          _timer = new Timer(_ => { _hubConnection.SendAsync("CalculateTimeDifference", DateTime.Now); }, null, 0,
            1000);
        });

      _hubConnection.On("EndConversation", async () =>
      {
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

      _hubConnection.On("ParticipantAgreedToContinue", () =>
      {
        _dispatcher.Dispatch(new ParticipantAgreedToContinue());

        if (_chatState.Value.UserContinue && _chatState.Value.ParticipantContinue)
        {
          _dispatcher.Dispatch(new EndConversation());
          GoToConversation();
        }
      });
    }

    protected override async void OnAfterRender(bool firstRender)
    {
      if (firstRender)
      {
        await _hubConnection.SendAsync("QueueForConversation");
      }
    }

    private async Task Send(string input)
    {
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

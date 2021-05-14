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
using Microsoft.AspNetCore.SignalR.Client;

namespace Jessamine.Client.Pages
{
  [Authorize]
  public partial class Chat
  {
    private HubConnection _hubConnection;

    private readonly List<string> _messages = new List<string>();
    private Timer _timer;

    private TimeSpan TimeToEnd => _chatState.Value.TimeToEnd;

    private string _userName;

    protected override async Task OnInitializedAsync()
    {
      var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
      var user = authState.User;

      _userName = user.Identity.Name;

      Console.WriteLine($"ChatHub uri: {NavigationManager.BaseUri}, URI: {NavigationManager.Uri}");

      _hubConnection = new HubConnectionBuilder()
        .WithUrl(NavigationManager.ToAbsoluteUri("/chathub"), options =>
        {
          options.AccessTokenProvider = async () =>
          {
            var accessTokenResult = await AccessTokenProvider.RequestAccessToken();

            Console.WriteLine(accessTokenResult.Status);

            accessTokenResult.TryGetToken(out var accessToken);
            return accessToken.Value;
          };
        })
        .Build();

      _hubConnection.On<Message>("ReceiveMessage", (message) =>
      {
        _dispatcher.Dispatch(new ReceiveMessage(message));
      });
       
      _hubConnection.On<bool, string, long>("ConnectWithUser", (isConnected, connectedUserConnectionId, conversationId) =>
      {
        _dispatcher.Dispatch(new StartConversation(connectedUserConnectionId, isConnected, conversationId));

        _timer = new Timer(_ =>
        {
          _hubConnection.SendAsync("CalculateTimeDifference", DateTime.Now);
        }, null, 0,1000);
      });
      
      _hubConnection.On("EndConversation", async () =>
      {
        _dispatcher.Dispatch(new EndConversation());
        _dispatcher.Dispatch(new ClearMessenger());

        await _hubConnection.SendAsync("QueueForConversation");
      });

      _hubConnection.On<long, bool>("SetTimer", (timeElapsed, timeHasRunOut) =>
      {
        if (timeHasRunOut)
        {
          _timer.Dispose();
          return;
        }

        _dispatcher.Dispatch(new SetTimer(timeElapsed));
      });

      await _hubConnection.StartAsync();
    }

    protected override async void OnAfterRender(bool firstRender)
    {
      if (firstRender)
      {
        await _hubConnection.SendAsync("QueueForConversation");
      }
    }

    async Task Send(string input)
    {
      await _hubConnection.SendAsync("SendMessage", _chatState.Value.ConnectedUserId, input,
        _chatState.Value.ConversationId);

      _dispatcher.Dispatch(new ChangeInput(string.Empty));
    }
  
    public bool IsConnected => _chatState.Value.IsConnected;

    public async ValueTask DisposeAsync()
    {
      await _hubConnection.DisposeAsync();
      _dispatcher.Dispatch(new ClearMessenger());
    }
  }
}

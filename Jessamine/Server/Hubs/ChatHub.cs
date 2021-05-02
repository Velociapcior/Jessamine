using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jessamine.Server.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Jessamine.Server.Hubs
{
  [Authorize]
  public class ChatHub : Hub
  {
    private readonly IPairingProvider _pairingProvider;

    public ChatHub(IPairingProvider pairingProvider)
    {
      _pairingProvider = pairingProvider;
    }

    public async Task QueueForConversation()
    {
      try
      {
        var isSuccessful = _pairingProvider.PairUser(Context.ConnectionId);

        if (isSuccessful)
        {
          string pairedUserConnectionId = _pairingProvider.FindPair(Context.ConnectionId);

          await Clients.Client(pairedUserConnectionId).SendAsync("ConnectWithUser", true, Context.ConnectionId);

          await Clients.Client(Context.ConnectionId).SendAsync("ConnectWithUser", true, pairedUserConnectionId);
        }
      }
      catch (Exception e)
      {
        throw;
      }
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
      var pair = _pairingProvider.RemovePair(Context.ConnectionId);

      if (pair != null)
      {
        Clients.Clients(new[]{ pair.FirstUser, pair.SecondUser }).SendAsync("EndConversation");
      }
      
      return base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessage(string user, string message)
    {
      await Clients.All.SendAsync("ReceiveMessage", user, message);
    }
  }
}

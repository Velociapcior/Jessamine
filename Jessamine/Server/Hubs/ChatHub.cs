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

    public override Task OnConnectedAsync()
    {
      _pairingProvider.PairUser(Context.ConnectionId);

      return base.OnConnectedAsync();
    }

    public async Task SendMessage(string user, string message)
    {
      await Clients.All.SendAsync("ReceiveMessage", user, message);
    }
  }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using IdentityServer4.Stores;
using Jessamine.Server.Data;
using Jessamine.Server.Models;
using Jessamine.Server.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

namespace Jessamine.Server.Hubs
{
  [Authorize]
  public class ChatHub : Hub
  {
    private readonly TimeSpan _defaultConvesationTime = new TimeSpan(0, 0, 5, 0);

    private readonly IPairingProvider _pairingProvider;
    private readonly IConnectionMapping<string> _connections;

    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;

    public ChatHub(IPairingProvider pairingProvider,
      IConnectionMapping<string> connections,
      UserManager<ApplicationUser> userManager,
      ApplicationDbContext context)
    {
      _pairingProvider = pairingProvider;
      _connections = connections;
      _userManager = userManager;
      _context = context;
    }

    public async Task QueueForConversation()
    {
      try
      {
        var isSuccessful = _pairingProvider.PairUser(Context.ConnectionId);

        if (isSuccessful)
        {
          string pairedUserConnectionId = _pairingProvider.FindPairedUser(Context.ConnectionId);

          var participants = new List<ApplicationUser>();

          participants.Add(_userManager.Users.First(x => x.Id == Context.UserIdentifier));
          participants.Add(_userManager.Users.First(x => x.Id == _connections.GetUser(pairedUserConnectionId)));

          var conversation = _context.Conversations.Add(new Conversation
          {
            Messages = new List<Message>(),
            Participants = participants,
            StartedDate = DateTime.Now
          });
          
          await _context.SaveChangesAsync();

          _pairingProvider.SetConversation(Context.ConnectionId, pairedUserConnectionId, conversation.Entity.Id, conversation.Entity.StartedDate);

          await Clients.Client(pairedUserConnectionId).SendAsync("ConnectWithUser", true, Context.ConnectionId, conversation.Entity.Id);

          await Clients.Client(Context.ConnectionId).SendAsync("ConnectWithUser", true, pairedUserConnectionId, conversation.Entity.Id);
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

      string userIdentifier = Context.UserIdentifier;

      _connections.Remove(userIdentifier, Context.ConnectionId);

      return base.OnDisconnectedAsync(exception);
    }

    public override Task OnConnectedAsync()
    {
      string userIdentifier = Context.UserIdentifier;

      _connections.Add(userIdentifier, Context.ConnectionId);

      return base.OnConnectedAsync();
    }

    public async Task CalculateTimeDifference(DateTime curerntTime)
    {
      var pair = _pairingProvider.FindPair(Context.ConnectionId);

      TimeSpan elapsedTime = curerntTime - pair.ConversationStartDate;

      var timeLeft = _defaultConvesationTime - elapsedTime;
      
      await Clients.Clients(Context.ConnectionId).SendAsync("SetTimer", timeLeft.Ticks, elapsedTime > _defaultConvesationTime);
    }

    public async Task SendMessage(string connectionId, string content, long conversationId)
    {
      var from = await _userManager.GetUserAsync(Context.User);
      var to = _userManager.Users.Single(x => x.Id == _connections.GetUser(connectionId));

      var conversation = await _context.Conversations.FindAsync(conversationId);

      Message messageDto = new Message
      {
        Content = content,
        Date = DateTime.Now,
        From = from.UserName,
        To = to.UserName,
        Conversation = conversation
      };

      var entityMessage = _context.Messages.Add(messageDto);

      await _context.SaveChangesAsync();

      var message = new Shared.Message()
      {
        Content = entityMessage.Entity.Content,
        Date = entityMessage.Entity.Date,
        From = entityMessage.Entity.From,
        Id = entityMessage.Entity.Id,
        To = entityMessage.Entity.To
      };

      await Clients.Caller.SendAsync("ReceiveMessage", message);
      await Clients.Client(connectionId).SendAsync("ReceiveMessage", message);
    }
  }
}

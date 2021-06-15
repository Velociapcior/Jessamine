using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer4.Stores;
using Jessamine.Server.Data;
using Jessamine.Server.Extensions;
using Jessamine.Server.Models;
using Jessamine.Server.Services.Converters.Interfaces;
using Jessamine.Server.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;

namespace Jessamine.Server.Hubs
{
  [Authorize]
  public class ChatHub : Hub
  {
    private readonly TimeSpan _defaultConvesationTime = TimeSpan.FromSeconds(30);

    private readonly IPairingProvider _pairingProvider;
    private readonly IConnectionMapping<string> _connections;
    private readonly IMessageConverter _messageConverter;
    private readonly ILogger<ChatHub> _logger;

    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;

    public ChatHub(
      IPairingProvider pairingProvider,
      IConnectionMapping<string> connections,
      UserManager<ApplicationUser> userManager,
      ApplicationDbContext context, 
      IMessageConverter messageConverter, 
      ILogger<ChatHub> logger)
    {
      _pairingProvider = pairingProvider;
      _connections = connections;
      _userManager = userManager;
      _context = context;
      _messageConverter = messageConverter;
      _logger = logger;
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

          var currentUser = await _userManager.GetUserAsync(Context.User);
          var pairedUser = await _userManager.FindByIdAsync(_connections.GetUser(pairedUserConnectionId));

          participants.Add(currentUser);
          participants.Add(pairedUser);

          var conversation = _context.Conversations.Add(new Conversation
          {
            Messages = new List<Message>(),
            Participants = participants,
            StartedDate = DateTime.Now
          });
          
          await _context.SaveChangesAsync();

          _pairingProvider.SetConversation(Context.ConnectionId, pairedUserConnectionId, conversation.Entity.Id, conversation.Entity.StartedDate);

          var clientTask = Clients.Client(pairedUserConnectionId).SendAsync("ConnectWithUser", true, Context.ConnectionId, conversation.Entity.Id, currentUser.UserName);

          var callerTask = Clients.Client(Context.ConnectionId).SendAsync("ConnectWithUser", true, pairedUserConnectionId, conversation.Entity.Id, pairedUser.UserName);

          await TaskExt.WhenAll(callerTask, clientTask);
        }
      }
      catch (Exception e)
      {
        _logger.LogCritical(e, $"Failed to connect user: {Context.UserIdentifier} with another user");
        throw;
      }
    }

    public async Task AcceptConversation(long conversationId)
    {
      var conversation = await _context.Conversations.FindAsync(conversationId);

      conversation.Accepted = true;

      await _context.SaveChangesAsync();
    }

    public async Task EndConversation(long conversationId)
    {
      var pair = _pairingProvider.RemovePair(Context.ConnectionId);

      var conversation = await _context.Conversations.FindAsync(conversationId);

      conversation.Accepted = false;

      await _context.SaveChangesAsync();

      if (pair != null)
      {
        await Clients.Clients(new[] { pair.FirstUser, pair.SecondUser }).SendAsync("EndConversation");
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

      conversation.LastMessage = content;
      conversation.LastMessageDate = messageDto.Date;
      conversation.LastMessageStatus = 3;

      var entityMessage = _context.Messages.Add(messageDto);

      await _context.SaveChangesAsync();

      var message = _messageConverter.Map(entityMessage.Entity);

      var callerTask = Clients.Caller.SendAsync("ReceiveMessage", message);
      var clientTask = Clients.Client(connectionId).SendAsync("ReceiveMessage", message);

      await TaskExt.WhenAll(callerTask, clientTask);
    }

    public async Task ParticipantAgreedToContinue(string connectedUserId)
    {
      await Clients.Clients(connectedUserId).SendAsync("ParticipantAgreedToContinue");
    }
  }
}

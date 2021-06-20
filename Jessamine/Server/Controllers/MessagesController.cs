using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Validation;
using Jessamine.Server.Data;
using Jessamine.Server.Models;
using Jessamine.Server.Services.Converters.Interfaces;
using Jessamine.Shared.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Message = Jessamine.Shared.Message;

namespace Jessamine.Server.Controllers
{
  [Authorize]
  [ApiController]
  [Route("api/[controller]")]
  public class MessagesController : ControllerBase
  {
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMessageConverter _messageConverter;
    private readonly ILogger<MessagesController> _logger;

    public MessagesController(
      ApplicationDbContext context,
      UserManager<ApplicationUser> userManager,
      IMessageConverter messageConverter,
      ILogger<MessagesController> logger)
    {
      _context = context;
      _userManager = userManager;
      _messageConverter = messageConverter;
      _logger = logger;
    }

    [HttpGet]
    public async Task<IEnumerable<Shared.Message>> Get([FromQuery] long conversationId)
    {
      var user = await _userManager.GetUserAsync(HttpContext.User);

      var entityMessages =
        _context.Messages.Where(x =>
          x.Conversation.Id == conversationId &&
          x.Conversation.Participants.Contains(user))
        .Include(x => x.Conversation);

      var messages = entityMessages.Select(e => _messageConverter.Map(e));

      return messages;
    }

    [HttpGet]
    [Route("new")]
    public async Task<IEnumerable<Message>> GetNewMessages([FromQuery] long mesageId, [FromQuery] long conversationId)
    {
      var user = await _userManager.GetUserAsync(HttpContext.User);

      var entityMessages =
        _context.Messages
          .Where(x =>
            x.Conversation.Id == conversationId &&
            x.Conversation.Participants.Contains(user))
          .Include(x => x.Conversation)
          .AsEnumerable()
          .SkipWhile(x => x.Id <= mesageId);

      var messages = entityMessages.Select(e => _messageConverter.Map(e));

      return messages;
    }

    [HttpPost]
    public async Task<IActionResult> Post(Shared.Message message)
    {
      await using IDbContextTransaction transaction = await _context.Database.BeginTransactionAsync();
      try
      {
        var messageEntity = _messageConverter.Map(message);
        var conversation = await _context.Conversations.FindAsync(message.ConversationId);

        conversation.LastMessage = message.Content;
        conversation.LastMessageDate = message.Date;
        conversation.LastMessageStatus = (int)MessageStatus.Sent;

        messageEntity.Conversation = conversation;
        var entry = _context.Messages.Add(messageEntity);

        await _context.SaveChangesAsync();
        await transaction.CommitAsync();

        return new JsonResult(entry.Entity.Id);
      }
      catch (Exception e)
      {
        await transaction.RollbackAsync();
        _logger.LogError(e, $"Error while adding message {message.ConversationId}");
        return BadRequest();
      }
    }

  }
}

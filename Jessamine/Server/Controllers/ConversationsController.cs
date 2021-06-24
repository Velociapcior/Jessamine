using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jessamine.Server.Data;
using Jessamine.Server.Data.Models;
using Jessamine.Server.Services.Converters.Interfaces;
using Jessamine.Shared.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace Jessamine.Server.Controllers
{
  [Authorize]
  [ApiController]
  [Route("api/[controller]")]
  public class ConversationsController : ControllerBase
  {

    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;
    private readonly IConversationConverter _conversationConverter;
    private readonly ILogger<ConversationsController> _logger;

    public ConversationsController(
      ApplicationDbContext context,
      UserManager<ApplicationUser> userManager,
      IConversationConverter conversationConverter,
      ILogger<ConversationsController> logger)
    {
      _context = context;
      _userManager = userManager;
      _conversationConverter = conversationConverter;
      _logger = logger;
    }

    [HttpGet]
    public async Task<IEnumerable<Shared.Conversation>> Get()
    {
      await using IDbContextTransaction transaction = await _context.Database.BeginTransactionAsync();
      try
      {
        ApplicationUser user = await _userManager.GetUserAsync(User);

        var conversationEntities = _context
          .Conversations
          .Where(c => c.Participants.Contains(user) && c.Accepted)
          .Include(x => x.Participants)
          .OrderByDescending(x => x.LastMessageDate);

        await conversationEntities
          .Where(x =>
            x.LastMessageStatus == (int)MessageStatus.Sent && 
            x.Messages.OrderBy(y => y.Date).Last()
              .From != user.UserName)
          .ForEachAsync(x => x.LastMessageStatus = (int)MessageStatus.Received);

        var conversations = conversationEntities.Select(
          x => _conversationConverter
            .Map(x, x.Participants.Single(y => y.Id != user.Id).UserName));

        await _context.SaveChangesAsync();
        await transaction.CommitAsync();

        return conversations;
      }
      catch (Exception ex)
      {
        await transaction.RollbackAsync();
        _logger.LogCritical(ex, $"Error while getting conversation list, user {HttpContext.User.Identity?.Name}");
        throw;
      }
    }

    [HttpPatch]
    [Route("{id}")]
    public async Task<IActionResult> Patch(long id, Shared.Conversation conversation)
    {
      await using IDbContextTransaction transaction = await _context.Database.BeginTransactionAsync();
      try
      {
        var conversationEntity = await _context.Conversations.FindAsync(id);

        if (conversationEntity == null)
        {
          return NotFound();
        }

        conversationEntity.LastMessageStatus = (int)conversation.LastMessageStatus;

        await _context.SaveChangesAsync();
        await transaction.CommitAsync();

        return Ok();
      }
      catch (Exception e)
      {
        await transaction.RollbackAsync();
        _logger.LogCritical(e, $"Error while updating conversation last message status, user {HttpContext.User.Identity?.Name}");
        throw;
      }
    }
  }
}

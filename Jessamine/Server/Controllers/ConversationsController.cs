using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jessamine.Server.Data;
using Jessamine.Server.Models;
using Jessamine.Server.Services.Converters.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
      try
      {
        ApplicationUser user = await _userManager.GetUserAsync(User);

        var conversationEntities = _context
          .Conversations
          .Where(c => c.Participants.Contains(user) && c.Accepted)
          .Include(x => x.Participants)
          .OrderByDescending(x => x.LastMessageDate);


        var conversations =
          conversationEntities
            .Select(x => 
              _conversationConverter.Map(
                x, 
                x.Participants
                .Single(y =>
                  y.Id != user.Id).UserName));

        return conversations;
      }
      catch (Exception ex)
      {
        _logger.LogCritical(ex, $"Error while getting conversation list, user {HttpContext.User.Identity?.Name}");
        throw;
      }
    }
  }
}

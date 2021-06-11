using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Validation;
using Jessamine.Server.Data;
using Jessamine.Server.Models;
using Jessamine.Server.Services.Converters.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

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

    public MessagesController(
      ApplicationDbContext context,
      UserManager<ApplicationUser> userManager,
      IMessageConverter messageConverter)
    {
      _context = context;
      _userManager = userManager;
      _messageConverter = messageConverter;
    }

    [HttpGet]
    public async Task<IEnumerable<Shared.Message>> Get([FromQuery] int conversationId)
    {
      var user = await _userManager.GetUserAsync(HttpContext.User);

      var entityMessages =
        _context.Messages.Where(x =>
          x.Conversation.Id == conversationId &&
          x.Conversation.Participants.Contains(user));

      var messages = entityMessages.Select(e => _messageConverter.Map(e));

      return messages;
    }
  }
}

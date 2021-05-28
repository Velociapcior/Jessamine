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

    public ConversationsController(
      ApplicationDbContext context,
      UserManager<ApplicationUser> userManager,
      IConversationConverter conversationConverter)
    {
      _context = context;
      _userManager = userManager;
      _conversationConverter = conversationConverter;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
      ApplicationUser user = await _userManager.GetUserAsync(User);

      var conversationEntities = _context
        .Conversations
        .Where(c => c.Participants.Contains(user) && c.Accepted)
        .OrderByDescending(x => x.LastMessageDate);


      var conversations =
        conversationEntities.Select(x => _conversationConverter.Map(x, x.Participants.Single(y => y.Id != user.Id).UserName));

      return new JsonResult(conversations);
    }
  }
}

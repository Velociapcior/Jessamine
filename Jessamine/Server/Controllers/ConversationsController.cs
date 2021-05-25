using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jessamine.Server.Data;
using Jessamine.Server.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Jessamine.Server.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class ConversationsController : ControllerBase
  {

    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;

    public ConversationsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
      _context = context;
      _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
      ApplicationUser user = await _userManager.GetUserAsync(User);

      var conversations = _context
        .Conversations
        .Where(c => c.Participants.Contains(user) && c.Accepted)
        .OrderByDescending(x => x.LastMessageDate);

      return new JsonResult(conversations);
    }
  }
}

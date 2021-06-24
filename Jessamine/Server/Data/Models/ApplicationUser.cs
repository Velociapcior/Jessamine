using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Jessamine.Server.Data.Models
{
  public class ApplicationUser : IdentityUser
  {
    public virtual ICollection<Conversation> Conversations { get; set; }
  }

}

using Microsoft.AspNetCore.Identity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Jessamine.Server.Models
{
  public class ApplicationUser : IdentityUser
  {
    public virtual ICollection<Conversation> Conversations { get; set; }
  }

}

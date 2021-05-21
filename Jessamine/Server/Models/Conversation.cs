using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jessamine.Server.Models
{
  public class Conversation
  {
    public long Id { get; set; }

    public virtual ICollection<ApplicationUser> Participants { get; set; }

    public virtual ICollection<Message> Messages { get; set; }

    public bool Accepted { get; set; }

    public DateTime StartedDate { get; set; }
  }
}

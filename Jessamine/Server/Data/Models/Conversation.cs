using System;
using System.Collections.Generic;

namespace Jessamine.Server.Data.Models
{
  public class Conversation
  {
    public long Id { get; set; }

    public virtual ICollection<ApplicationUser> Participants { get; set; }

    public virtual ICollection<Message> Messages { get; set; }

    public bool Accepted { get; set; }

    public DateTime StartedDate { get; set; }

    public string LastMessage { get; set; }

    public DateTime LastMessageDate { get; set; }

    public int LastMessageStatus { get; set; }
  }
}

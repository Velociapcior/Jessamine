using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jessamine.Server.Models
{
  public class Message
  {
    public long Id { get; set; }

    public string From { get; set; }

    public string To { get; set; }

    public DateTime Date { get; set; }

    public string Content { get; set; }

    public virtual Conversation Conversation { get; set; }
  }
}

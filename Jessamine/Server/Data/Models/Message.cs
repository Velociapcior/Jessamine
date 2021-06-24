using System;

namespace Jessamine.Server.Data.Models
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

using System;

namespace Jessamine.Shared
{
  public class Message
  {
    public long Id { get; set; }

    public string From { get; set; }

    public string To { get; set; }

    public DateTime Date { get; set; }

    public string Content { get; set; }

    public long ConversationId { get; set; }
  }
}

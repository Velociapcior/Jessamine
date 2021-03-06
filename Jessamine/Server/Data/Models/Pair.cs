using System;

namespace Jessamine.Server.Data.Models
{
  public class Pair
  {
    public Pair(string firstUser)
    {
      FirstUser = firstUser;
    }

    public long ConversationId { get; set; }

    public DateTime ConversationStartDate { get; set; }

    public string FirstUser { get; set; }

    public string SecondUser { get; set; }
  }
}

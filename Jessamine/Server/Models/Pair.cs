using System;

namespace Jessamine.Server.Models
{
  public class Pair
  {
    public Pair(string firstUser)
    {
      FirstUser = firstUser;
    }

    public string FirstUser { get; set; }

    public string SecondUser { get; set; }
  }
}

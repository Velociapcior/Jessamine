using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jessamine.Shared
{
  public class Message
  {
    public long Id { get; set; }

    public string From { get; set; }

    public string To { get; set; }

    public DateTime Date { get; set; }

    public string Content { get; set; }
  }
}

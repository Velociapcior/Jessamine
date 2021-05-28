using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jessamine.Shared.Common;

namespace Jessamine.Shared
{
  public class Conversation
  {
    public long Id { get; set; }

    public string ParticipantName { get; set; }

    public MessageStatus LastMessageStatus { get; set; }

    public DateTime StartedDate { get; set; }

    public string LastMessage { get; set; }

    public DateTime LastMessageDate { get; set; }
  }
}

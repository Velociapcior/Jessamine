using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jessamine.Server.Services.Converters.Interfaces;
using Jessamine.Shared;
using Jessamine.Shared.Common;

namespace Jessamine.Server.Services.Converters
{
  public class ConversationConverter : IConversationConverter
  {
    public Conversation Map(Models.Conversation entity, string participantName)
    {
      Conversation conversation = new Conversation
      {
        Id = entity.Id,
        LastMessage = entity.LastMessage,
        LastMessageDate = entity.LastMessageDate,
        LastMessageStatus = (MessageStatus) entity.LastMessageStatus,
        ParticipantName = participantName,
        StartedDate = entity.StartedDate
      };

      return conversation;
    }
  }
}

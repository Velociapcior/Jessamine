using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jessamine.Server.Models;
using Jessamine.Server.Services.Converters.Interfaces;
using Message = Jessamine.Shared.Message;

namespace Jessamine.Server.Services.Converters
{
  public class MessageConverter : IMessageConverter
  {
    public Message Map(Models.Message entity)
    {
      var message = new Message
      {
        Content = entity.Content,
        Date = entity.Date,
        From = entity.From,
        Id = entity.Id,
        To = entity.To,
        ConversationId = entity.Conversation.Id
      };

      return message;
    }

    public Models.Message Map(Message model)
    {
      var message = new Models.Message
      {
        Content = model.Content,
        Date = model.Date,
        From = model.From,
        Id = model.Id,
        To = model.To,
        Conversation = new Conversation { Id = model.ConversationId }
      };

      return message;
    }
  }
}

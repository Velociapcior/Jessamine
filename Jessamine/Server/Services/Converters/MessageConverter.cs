using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jessamine.Server.Services.Converters.Interfaces;
using Jessamine.Shared;

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
        To = entity.To
      };

      return message;
    }
  }
}

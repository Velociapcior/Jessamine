using Jessamine.Shared;

namespace Jessamine.Server.Services.Converters.Interfaces
{
  public interface IMessageConverter
  {
    Message Map(Data.Models.Message entity);

    Data.Models.Message Map(Message model);
  }
}
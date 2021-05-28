using Jessamine.Shared;

namespace Jessamine.Server.Services.Converters.Interfaces
{
  public interface IMessageConverter
  {
    Message Map(Models.Message entity);
  }
}
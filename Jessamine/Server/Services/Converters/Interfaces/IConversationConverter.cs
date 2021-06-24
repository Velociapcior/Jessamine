using Jessamine.Shared;

namespace Jessamine.Server.Services.Converters.Interfaces
{
  public interface IConversationConverter
  {
    Conversation Map(Data.Models.Conversation entity, string participantName);

    Data.Models.Conversation Map(Conversation conversation);
  }
}
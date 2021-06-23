using Jessamine.Shared;

namespace Jessamine.Server.Services.Converters.Interfaces
{
  public interface IConversationConverter
  {
    Conversation Map(Models.Conversation entity, string participantName);

    Models.Conversation Map(Conversation conversation);
  }
}
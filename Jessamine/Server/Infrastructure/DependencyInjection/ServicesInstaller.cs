using System;
using IdentityServer4.Services;
using Jessamine.Server.Services;
using Jessamine.Server.Services.Converters;
using Jessamine.Server.Services.Converters.Interfaces;
using Jessamine.Server.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Jessamine.Server.Infrastructure.DependencyInjection
{
  public static class ServicesInstaller
  {
    public static void Install(IServiceCollection services)
    {
      services.AddSingleton<IMessageConverter, MessageConverter>();
      services.AddSingleton<IConversationConverter, ConversationConverter>();
    }
  }
}

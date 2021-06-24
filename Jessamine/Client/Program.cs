using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Fluxor;
using Humanizer.Configuration;
using Humanizer.DateTimeHumanizeStrategy;

namespace Jessamine.Client
{
  public class Program
  {
    public static async Task Main(string[] args)
    {
      var builder = WebAssemblyHostBuilder.CreateDefault(args);
      builder.RootComponents.Add<App>("#app");

      builder.Logging.SetMinimumLevel(LogLevel.Warning);

      builder.Services.AddHttpClient("Jessamine.ServerAPI", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
          .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

      // Supply HttpClient instances that include access tokens when making requests to the server project
      builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("Jessamine.ServerAPI"));

      builder.Services.AddApiAuthorization();

      builder.Services.AddFluxor(config =>
      {
        config.ScanAssemblies(typeof(Program).Assembly);
        config.UseReduxDevTools();
      });

      Configurator.DateTimeHumanizeStrategy = new PrecisionDateTimeHumanizeStrategy(precision: .99);
      Configurator.DateTimeOffsetHumanizeStrategy = new PrecisionDateTimeOffsetHumanizeStrategy(precision: .99); // configure when humanizing DateTimeOffset
     
      await DebugDelayAsync();
      await builder.Build().RunAsync();
    }

    private static async Task DebugDelayAsync()
    {
#if DEBUG
      await Task.Delay(5000);
#endif
    }
  }
}

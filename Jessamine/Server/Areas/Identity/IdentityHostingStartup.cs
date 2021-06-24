using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(Jessamine.Server.Areas.Identity.IdentityHostingStartup))]
namespace Jessamine.Server.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
            });
        }
    }
}
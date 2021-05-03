using Jessamine.Server.Data;
using Jessamine.Server.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Linq;
using System.Security.Claims;
using Fluxor.DependencyInjection;
using IdentityServer4.Services;
using Jessamine.Server.Hubs;
using Jessamine.Server.Services;
using Jessamine.Server.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Jessamine.Server
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddDbContext<ApplicationDbContext>(options =>
          options.UseSqlServer(
              Configuration.GetConnectionString("DefaultConnection")));

      services.AddDatabaseDeveloperPageExceptionFilter();

      services.AddDefaultIdentity<ApplicationUser>(options =>
        {
          options.SignIn.RequireConfirmedAccount = true;
        })
          .AddEntityFrameworkStores<ApplicationDbContext>();

      services.AddIdentityServer()
          .AddApiAuthorization<ApplicationUser, ApplicationDbContext>(options =>
          {
            options.IdentityResources["openid"].UserClaims.Add("name");
            options.ApiResources.Single().UserClaims.Add("name");
            options.IdentityResources["openid"].UserClaims.Add("role");
            options.ApiResources.Single().UserClaims.Add("role");
          });

      services.AddAuthentication()
          .AddIdentityServerJwt();

      services.AddControllersWithViews();
      services.AddRazorPages();
      services.AddSignalR();
      services.AddResponseCompression(opts =>
      {
        opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
          new[] { "application/octet-stream" });
      });

      services.AddSingleton<IConnectionMapping<string>, ConnectionMapping<string>>();
      services.AddSingleton<IPairingProvider, PairingProvider>();
      services.AddTransient<IProfileService, ProfileService>();

      services.Configure<IdentityOptions>(options =>
      {
        options.ClaimsIdentity.UserIdClaimType = ClaimTypes.NameIdentifier;
      });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      app.UseResponseCompression();

      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
        app.UseMigrationsEndPoint();
        app.UseWebAssemblyDebugging();
      }
      else
      {
        app.UseExceptionHandler("/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
      }

      app.UseHttpsRedirection();
      app.UseBlazorFrameworkFiles();
      app.UseStaticFiles();

      app.UseRouting();

      app.UseIdentityServer();
      app.UseAuthentication();
      app.UseAuthorization();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapRazorPages();
        endpoints.MapControllers();
        endpoints.MapHub<ChatHub>("/chathub");
        endpoints.MapFallbackToFile("index.html");
      });
    }
  }
}

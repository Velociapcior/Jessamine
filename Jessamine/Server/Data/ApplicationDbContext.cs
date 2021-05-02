using IdentityServer4.EntityFramework.Options;
using Jessamine.Server.Models;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jessamine.Server.Data
{
  public class ApplicationDbContext : ApiAuthorizationDbContext<ApplicationUser>
  {
    public ApplicationDbContext(
        DbContextOptions options,
        IOptions<OperationalStoreOptions> operationalStoreOptions) : base(options, operationalStoreOptions)
    {
    }

    public DbSet<Conversation> Conversations { get; set; }

    public DbSet<Message> Messages { get; set; }
  }
}

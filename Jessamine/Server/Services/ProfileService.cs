using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Services;

namespace Jessamine.Server.Services
{
  public class ProfileService : IProfileService
  {
    public ProfileService()
    {
    }

    public async Task GetProfileDataAsync(ProfileDataRequestContext context)
    {
      var nameClaim = context.Subject.FindAll(JwtClaimTypes.Name);
      context.IssuedClaims.AddRange(nameClaim);

      var givenNameClaim = context.Subject.FindAll(JwtClaimTypes.GivenName);
      context.IssuedClaims.AddRange(givenNameClaim);

      var roleClaims = context.Subject.FindAll(JwtClaimTypes.Role);
      context.IssuedClaims.AddRange(roleClaims);

      await Task.CompletedTask;
    }

    public async Task IsActiveAsync(IsActiveContext context)
    {
      await Task.CompletedTask;
    }
  }
}
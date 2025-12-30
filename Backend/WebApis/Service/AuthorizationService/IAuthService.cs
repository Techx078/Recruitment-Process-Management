using System.Security.Claims;

namespace WebApis.Service.AuthorizationService
{
    public interface IAuthService
    {
        Task ValidateHrLevelAccessAsync(int jobOpeningId, ClaimsPrincipal user);
    }
}

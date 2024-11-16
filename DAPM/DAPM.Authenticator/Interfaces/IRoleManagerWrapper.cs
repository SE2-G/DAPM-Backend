using DAPM.Authenticator.Models;
using Microsoft.AspNetCore.Identity;

namespace DAPM.Authenticator.Interfaces
{
    public interface IRoleManagerWrapper
    {
        Task<IdentityResult> CreateAsync(Role role);
        Task<bool> RoleExistsAsync(string role);

    }
}

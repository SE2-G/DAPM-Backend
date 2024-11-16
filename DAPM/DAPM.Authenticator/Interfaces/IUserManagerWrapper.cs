using DAPM.Authenticator.Models;
using Microsoft.AspNetCore.Identity;

namespace DAPM.Authenticator.Interfaces
{
    public interface IUserManagerWrapper
    {
        Task<User> FindByNameAsync(string name);
        Task<IdentityResult> AddToRoleAsync(User user, string role);
        Task<IdentityResult> CreateAsync(User user, string password);
        Task<IList<string>> GetRolesAsync(User user);
    }
}
using DAPM.Authenticator.Interfaces;
using DAPM.Authenticator.Models;
using Microsoft.AspNetCore.Identity;

namespace DAPM.Authenticator.Data
{
    public class UserManagerWrapper : IUserManagerWrapper
    {
        UserManager<User> _userManager;

        public UserManagerWrapper(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public Task<IdentityResult> AddToRoleAsync(User user, string role)
        {
            return _userManager.AddToRoleAsync(user, role);
        }

        public Task<IdentityResult> CreateAsync(User user, string password)
        {
            return _userManager.CreateAsync(user, password);
        }

        public Task<User> FindByNameAsync(string name)
        {
            return _userManager.FindByNameAsync(name);
        }

        public Task<IList<string>> GetRolesAsync(User user)
        {
            return _userManager.GetRolesAsync(user);
        }
    }
}

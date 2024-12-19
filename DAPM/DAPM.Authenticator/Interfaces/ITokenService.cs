// Author: s205135

using DAPM.Authenticator.Models;

namespace DAPM.Authenticator.Interfaces
{
    public interface ITokenService
    {
        public Task<string> CreateToken(User user);
    }
}

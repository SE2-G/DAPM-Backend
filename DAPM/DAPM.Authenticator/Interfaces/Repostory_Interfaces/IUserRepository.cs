using DAPM.Authenticator.Models;

namespace DAPM.Authenticator.Interfaces.Repostory_Interfaces
{
    public interface IUserRepository
    {


        public Task<IEnumerable<User>> Users();

        public Task<bool> UserExists(string username);

        public Task<User> GetUserByName(string username);
        public Task<User> GetUserById(int id);

        public int SaveChanges(User user);
    }
}
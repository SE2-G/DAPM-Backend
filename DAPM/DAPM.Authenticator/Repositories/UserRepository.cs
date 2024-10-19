using DAPM.Authenticator.Data;
using DAPM.Authenticator.Interfaces.Repostory_Interfaces;
using DAPM.Authenticator.Models;

namespace DAPM.Authenticator.Repositories
{
    public class UserRepository : IUserRepository

    {
        private DataContext _datacontext;

        public UserRepository(DataContext dataContext)
        {
            _datacontext = dataContext;
        }

        public Task<User> GetUserById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<User> GetUserByName(string username)
        {
            throw new NotImplementedException();
        }

        public int SaveChanges(User user)
        {
            _datacontext.Update(user);
            return _datacontext.SaveChanges();
        }

        public Task<bool> UserExists(string username)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<User>> Users()
        {
            throw new NotImplementedException();
        }
    }
}

using UtilLibrary;

namespace DAPM.ClientApi.Services.Interfaces
{
    public interface IAuthenticatorService
    {
        public void SignUp();
        public Guid Login(LoginDto loginDto);
        public Guid RegisterUser(RegistrationDto registerDto);
        public void RemoveUser();
    }
}

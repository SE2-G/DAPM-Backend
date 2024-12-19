using RabbitMQLibrary.Models;

namespace DAPM.PeerApi.Services.Interfaces
{
    public interface IPeerUserService
    {
        public void OnAuthenticateRequest(Guid authenticationId, IdentityDTO senderIdentity, string UserName, string Password);
        public void OnAuthenticateRequestResponse(Guid authenticationId, IdentityDTO senderIdentity, string Passtoken, bool IsAuthenticated);
    }
}

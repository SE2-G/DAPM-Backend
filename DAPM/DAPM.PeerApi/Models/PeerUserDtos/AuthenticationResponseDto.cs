using RabbitMQLibrary.Models;

namespace DAPM.PeerApi.Models.PeerUserDtos
{
    public class AuthenticationResponseDto
    {
        public IdentityDTO SenderIdentity { get; set; }
        public Guid AuthenticationId { get; set; }
        public string UserName { get; set; }
        public string SessionToken { get; set; }
        public bool IsAuthenticated { get; set; }
    }
}

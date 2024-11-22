using RabbitMQLibrary.Models;

namespace DAPM.PeerApi.Models.PeerUserDtos
{
    public class AuthenticationResponseDto
    {
        public IdentityDTO SenderIdentity { get; set; }
        public Guid AuthenticationId { get; set; }
        public string PassToken { get; set; }
        public bool IsAuthenticated { get; set; }
    }
}

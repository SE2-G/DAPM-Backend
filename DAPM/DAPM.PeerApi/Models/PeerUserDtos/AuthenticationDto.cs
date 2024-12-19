using RabbitMQLibrary.Models;

namespace DAPM.PeerApi.Models.PeerUserDtos
{
    public class AuthenticationDto
    {
        public IdentityDTO SenderIdentity { get; set; }
        public Guid AuthenticationId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}

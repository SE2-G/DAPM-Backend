using RabbitMQLibrary.Models;

namespace DAPM.PeerApi.Models.PeerUserDtos
{
    public class InviteUserDto
    {
        public IdentityDTO SenderIdentity { get; set; }
        public Guid InviteUserId { get; set; }
        public string UserName { get; set; }
    }
}

using RabbitMQLibrary.Models;

namespace DAPM.PeerApi.Models.PeerUserDtos
{
    public class InviteUserResponseDto
    {
        public IdentityDTO SenderIdentity { get; set; }
        public Guid InviteUserId { get; set; }
        public bool IsAccepted { get; set; }
    }
}

using System.Text.Json.Serialization;

namespace DAPM.PeerApi.Models.PeerUserDtos
{
    public class PeerResponse
    {
        public string RequestName { get; set; }
        public Guid TicketId { get; set; }
    }
}

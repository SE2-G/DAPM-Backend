// Author: s224753

using RabbitMQLibrary.Interfaces;
using RabbitMQLibrary.Models;

namespace RabbitMQLibrary.Messages.Authenticator.Base
{
    public class PeerAuthenticateMessage : IQueueMessage
    {
        public Guid MessageId { get; set; }
        public Guid TicketId { get; set; }
        public Guid SenderProcessId { get; set; }
        public TimeSpan TimeToLive { get; set; }

        public string UserName { get; set; }
        public string Password { get; set; }
    }
}

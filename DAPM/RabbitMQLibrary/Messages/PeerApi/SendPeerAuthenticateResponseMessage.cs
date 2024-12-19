using RabbitMQLibrary.Interfaces;
using RabbitMQLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQLibrary.Messages.PeerApi.Handshake
{
    public class SendPeerAuthenticateResponseMessage : IQueueMessage
    {
        public Guid MessageId { get; set; }
        public Guid TicketId { get; set; }
        public Guid SenderProcessId { get; set; }
        public Guid authenticationId { get; set; }
        public TimeSpan TimeToLive { get; set; }

        public IdentityDTO SenderPeerIdentity { get; set; }

        public bool Succeeded { get; set; }
        public string UserName { get; set; }
        public string Passtoken { get; set; }
        public string Message { get; set; }
    }
}

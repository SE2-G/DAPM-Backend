using DAPM.PeerApi.Models.HandshakeDtos;
using DAPM.PeerApi.Services.Interfaces;
using RabbitMQLibrary.Interfaces;
using RabbitMQLibrary.Messages.PeerApi.Handshake;
using System.Text.Json;

namespace DAPM.PeerApi.Consumers
{
    public class SendPeerAuthenticateResponseConsumer : IQueueConsumer<SendPeerAuthenticateResponseMessage>
    {
        private IHttpService _httpService;
        public SendPeerAuthenticateResponseConsumer(IHttpService httpService)
        {
            _httpService = httpService;
        }

        public async Task ConsumeAsync(SendPeerAuthenticateResponseMessage message)
        {
            // var targetDomain = message.TargetDomain;
            var senderIdentity = message.SenderPeerIdentity;

            return;
        }
    }
}

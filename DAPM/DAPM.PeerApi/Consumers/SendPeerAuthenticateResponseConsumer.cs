using DAPM.PeerApi.Models.HandshakeDtos;
using DAPM.PeerApi.Models.PeerUserDtos;
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
            var targetDomain = message.SenderPeerIdentity.Domain;


            var authenticationResponseDto = new AuthenticationResponseDto()
            {
                AuthenticationId = message.authenticationId,
                IsAuthenticated = message.Succeeded,
                SessionToken = message.Passtoken,
                UserName = message.UserName,
            };

            var url = "http://" + targetDomain + PeerApiEndpoints.PeerAuthenticateRequestResponseEndpoint;
            var body = JsonSerializer.Serialize(authenticationResponseDto);

            var response = await _httpService.SendPostRequestAsync(url, body);

            return;
        }
    }
}

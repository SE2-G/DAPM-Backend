using DAPM.PeerApi.Services.Interfaces;
using RabbitMQLibrary.Interfaces;
using RabbitMQLibrary.Messages.Orchestrator.ProcessRequests;
using RabbitMQLibrary.Messages.Orchestrator.ServiceResults.FromPeerApi;
using RabbitMQLibrary.Models;

namespace DAPM.PeerApi.Services
{
    public class PeerUserService : IPeerUserService
    {
        private IQueueProducer<AuthenticateUserFromPeerRequest> _AuthenticateRequestProducer;
        private IQueueProducer<AuthenticateRequestResponseMessage> _AuthenticateRequestResponseProducer;

        public PeerUserService(IQueueProducer<AuthenticateUserFromPeerRequest> AuthenticateRequestProducer, 
            IQueueProducer<AuthenticateRequestResponseMessage> AuthenticateRequestResponseProducer,)
        {
            _AuthenticateRequestProducer = AuthenticateRequestProducer;
            _AuthenticateRequestResponseProducer = AuthenticateRequestResponseProducer;
        }

        void IPeerUserService.OnAuthenticateRequest(Guid authenticationId, IdentityDTO senderIdentity, string UserName, string Password)
        {
            var message = new AuthenticateUserFromPeerRequest()
            {
                TicketId = authenticationId,
                TimeToLive = TimeSpan.FromMinutes(1),
                SenderPeerIdentity = senderIdentity,
                UserName = UserName,
                Passtoken = Password,
            };

            _AuthenticateRequestProducer.PublishMessage(message);
        }

        void IPeerUserService.OnAuthenticateRequestResponse(Guid authenticationId, IdentityDTO senderIdentity, string Passtoken, bool IsAuthenticated)
        {
            var message = new AuthenticateRequestResponseMessage()
            {
                SenderProcessId = authenticationId,
                TimeToLive = TimeSpan.FromMinutes(1),
                SenderPeerIdentity = senderIdentity,
                Passtoken = Passtoken,
                IsAuthenticated = IsAuthenticated,
            };

            _AuthenticateRequestResponseProducer.PublishMessage(message);
        }
    }
}

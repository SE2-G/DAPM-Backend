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

        private IQueueProducer<InviteUserRequestMessage> _InviteUserRequestProducer;
        private IQueueProducer<InviteUserRequestResponseMessage> _InviteUserRequestResponseProducer;

        public PeerUserService(IQueueProducer<AuthenticateUserFromPeerRequest> AuthenticateRequestProducer, 
            IQueueProducer<AuthenticateRequestResponseMessage> AuthenticateRequestResponseProducer,
            IQueueProducer<InviteUserRequestMessage> InviteUserRequestProducer,
            IQueueProducer<InviteUserRequestResponseMessage> InviteUserRequestResponseProducer)
        {
            _AuthenticateRequestProducer = AuthenticateRequestProducer;
            _AuthenticateRequestResponseProducer = AuthenticateRequestResponseProducer;

            _InviteUserRequestProducer = InviteUserRequestProducer;
            _InviteUserRequestResponseProducer = InviteUserRequestResponseProducer;
        }

        void IPeerUserService.OnAuthenticateRequest(Guid authenticationId, IdentityDTO senderIdentity, string UserName, string Password)
        {
            var message = new AuthenticateUserFromPeerRequest()
            {
                SenderProcessId = authenticationId,
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

        void IPeerUserService.OnInviteUserRequest(Guid inviteUserId, IdentityDTO senderIdentity, string UserName)
        {
            var message = new InviteUserRequestMessage()
            {
                SenderProcessId = inviteUserId,
                TimeToLive = TimeSpan.FromMinutes(1),
                SenderPeerIdentity = senderIdentity,
                UserName = UserName,
            };

            _InviteUserRequestProducer.PublishMessage(message);
        }

        void IPeerUserService.OnInviteUserRequestResponse(Guid inviteUserId, IdentityDTO senderIdentity, bool IsAccepted)
        {
            var message = new InviteUserRequestResponseMessage()
            {
                SenderProcessId = inviteUserId,
                TimeToLive = TimeSpan.FromMinutes(1),
                SenderPeerIdentity = senderIdentity,
                IsAccepted = IsAccepted,
            };

            _InviteUserRequestResponseProducer.PublishMessage(message);
        }
    }
}

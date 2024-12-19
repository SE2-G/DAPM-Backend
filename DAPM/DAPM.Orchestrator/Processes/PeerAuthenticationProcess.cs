
using DAPM.Orchestrator.Services;
using DAPM.Orchestrator.Services.Models;
using RabbitMQLibrary.Interfaces;
using RabbitMQLibrary.Messages.Authenticator.Base;
using RabbitMQLibrary.Messages.ClientApi;
using RabbitMQLibrary.Messages.PeerApi.Handshake;
using RabbitMQLibrary.Models;

namespace DAPM.Orchestrator.Processes
{
    public class PeerAuthenticationProcess : OrchestratorProcess
    {
        private ILogger<PeerAuthenticationProcess> _logger;
        private IIdentityService _identityService;
        private Guid _ticketId;

        private IdentityDTO _senderPeerIdentity;
        private string _userName;
        private string _passtoken;

        public PeerAuthenticationProcess(OrchestratorEngine engine, IServiceProvider serviceProvider,
            Guid ticketId, Guid processId, IdentityDTO senderPeerIdentity, string userName, string passtoken) 
            : base(engine, serviceProvider, processId)
        {
            _identityService = serviceProvider.GetRequiredService<IIdentityService>();
            _logger = serviceProvider.GetRequiredService<ILogger<PeerAuthenticationProcess>>();
            _ticketId = ticketId;

            _senderPeerIdentity = senderPeerIdentity;
            _userName = userName;  
            _passtoken = passtoken;
        }

        public override void StartProcess()
        {
            _logger.LogInformation("AUTHENTICATION REQUESTED");
            var peerAuthenticateMessageProducer = _serviceScope.ServiceProvider.GetRequiredService<IQueueProducer<PeerAuthenticateMessage>>();

            var message = new PeerAuthenticateMessage()
            {
                SenderProcessId = _processId,
                TimeToLive = TimeSpan.FromMinutes(1),
                UserName = _userName,
                Password = _passtoken,
            };

            peerAuthenticateMessageProducer.PublishMessage(message);
        }

        public override void OnPeerAuthenticateResult(PeerAuthenticateResultMessage message)
        {
            _logger.LogInformation("SEND AUTHENTICATION REPONSE");
            var peerAuthenticateMessageProducer = _serviceScope.ServiceProvider.GetRequiredService<IQueueProducer<SendPeerAuthenticateResponseMessage>>();

            var message2 = new SendPeerAuthenticateResponseMessage()
            {
                SenderProcessId = _processId,
                TimeToLive = TimeSpan.FromMinutes(1),
                SenderPeerIdentity = _senderPeerIdentity,
                authenticationId = _ticketId,
                UserName = message.UserName,
                Passtoken = message.Passtoken,
                Succeeded = message.Succeeded,
                Message = message.Message,
            };

            peerAuthenticateMessageProducer.PublishMessage(message2);


        }
    }
}

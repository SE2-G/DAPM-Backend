
using DAPM.Orchestrator.Services;
using DAPM.Orchestrator.Services.Models;
using RabbitMQLibrary.Messages.ClientApi;
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
            throw new NotImplementedException();
        }

        public override void OnPeerAuthenticateResult(PeerAuthenticateResultMessage message)
        {

        }
    }
}

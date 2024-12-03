
using DAPM.Orchestrator.Services;
using DAPM.Orchestrator.Services.Models;

namespace DAPM.Orchestrator.Processes
{
    public class PeerInviteUserProcess : OrchestratorProcess
    {
        private ILogger<CollabHandshakeProcess> _logger;
        private Identity _localPeerIdentity;
        private Identity _requestedPeerIdentity;
        private IIdentityService _identityService;
        private string _requestedPeerDomain;
        private Guid _ticketId;

        public PeerInviteUserProcess(OrchestratorEngine engine, IServiceProvider serviceProvider,
            Guid ticketId, Guid processId, string requestedPeerDomain) 
            : base(engine, serviceProvider, processId)
        {
            _requestedPeerDomain = requestedPeerDomain;
            _identityService = serviceProvider.GetRequiredService<IIdentityService>();
            _localPeerIdentity = _identityService.GetIdentity();
            _logger = serviceProvider.GetRequiredService<ILogger<CollabHandshakeProcess>>();
            _ticketId = ticketId;
        }

        public override void StartProcess()
        {
            throw new NotImplementedException();
        }
    }
}

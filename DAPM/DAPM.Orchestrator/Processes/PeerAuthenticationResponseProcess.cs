
using DAPM.Orchestrator.Services;
using DAPM.Orchestrator.Services.Models;
using RabbitMQLibrary.Models;

namespace DAPM.Orchestrator.Processes
{
    public class PeerAuthenticationResponseProcess : OrchestratorProcess
    {
        private ILogger<PeerAuthenticationResponseProcess> _logger;
        private IIdentityService _identityService;
        private Guid _ticketId;

        private string _userName;
        private string _sessionToken;
        private bool _isAuthenticated;

        public PeerAuthenticationResponseProcess(OrchestratorEngine engine, IServiceProvider serviceProvider,
            Guid ticketId, Guid processId, IdentityDTO senderPeerIdentity, string userName, string sessionToken, bool IsAuthenticated) 
            : base(engine, serviceProvider, processId)
        {
            _identityService = serviceProvider.GetRequiredService<IIdentityService>();
            _logger = serviceProvider.GetRequiredService<ILogger<PeerAuthenticationResponseProcess>>();
            _ticketId = ticketId;

            _userName = userName;
            _sessionToken = sessionToken;
            _isAuthenticated = IsAuthenticated;
        }

        public override void StartProcess()
        {
            _logger.LogInformation("SESSIONTOKEN(" + _sessionToken +") RECIVED FOR USER: " + _userName);
        }
    }
}

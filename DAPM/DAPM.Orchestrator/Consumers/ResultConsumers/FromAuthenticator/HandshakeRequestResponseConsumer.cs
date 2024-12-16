using DAPM.Orchestrator.Processes;
using RabbitMQLibrary.Interfaces;
using RabbitMQLibrary.Messages.ClientApi;
using RabbitMQLibrary.Messages.Orchestrator.ServiceResults.FromPeerApi;

namespace DAPM.Orchestrator.Consumers.ResultConsumers.FromPeerApi
{
    public class PeerAuthenticateResultMessageConsumer : IQueueConsumer<PeerAuthenticateResultMessage>
    {
        private IOrchestratorEngine _orchestratorEngine;

        public PeerAuthenticateResultMessageConsumer(IOrchestratorEngine orchestratorEngine)
        {
            _orchestratorEngine = orchestratorEngine;
        }

        public Task ConsumeAsync(PeerAuthenticateResultMessage message)
        {
            OrchestratorProcess process = _orchestratorEngine.GetProcess(message.SenderProcessId);
            process.OnHandshakeRequestResponse(message);

            return Task.CompletedTask;
        }
    }
}

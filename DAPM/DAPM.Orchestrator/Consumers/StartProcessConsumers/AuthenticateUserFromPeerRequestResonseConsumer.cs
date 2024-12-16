using RabbitMQLibrary.Interfaces;
using RabbitMQLibrary.Messages.Orchestrator.ProcessRequests;
using RabbitMQLibrary.Messages.Orchestrator.ServiceResults.FromPeerApi;

namespace DAPM.Orchestrator.Consumers.StartProcessConsumers
{
    public class AuthenticateUserFromPeerRequestResonseConsumer : IQueueConsumer<AuthenticateRequestResponseMessage>
    {
        IOrchestratorEngine _engine;

        public AuthenticateUserFromPeerRequestResonseConsumer(IOrchestratorEngine engine)
        {
            _engine = engine;
        }

        public Task ConsumeAsync(AuthenticateRequestResponseMessage message)
        {
            _engine.StartAuthenticateUserFromPeerResponseProcess(message.MessageId, message.SenderPeerIdentity, message.UserName, message.Passtoken, message.IsAuthenticated);
            return Task.CompletedTask;
        }
    }
}

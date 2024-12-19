using RabbitMQLibrary.Interfaces;
using RabbitMQLibrary.Messages.Orchestrator.ProcessRequests;

namespace DAPM.Orchestrator.Consumers.StartProcessConsumers
{
    public class AuthenticateUserFromPeerRequestConsumer : IQueueConsumer<AuthenticateUserFromPeerRequest>
    {
        IOrchestratorEngine _engine;

        public AuthenticateUserFromPeerRequestConsumer(IOrchestratorEngine engine)
        {
            _engine = engine;
        }

        public Task ConsumeAsync(AuthenticateUserFromPeerRequest message)
        {
            _engine.StartAuthenticateUserFromPeerProcess(message.TicketId, message.SenderPeerIdentity, message.UserName, message.Passtoken);
            return Task.CompletedTask;
        }
    }
}

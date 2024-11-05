using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using RabbitMQLibrary.Interfaces;
using RabbitMQLibrary.Models;
using RabbitMQLibrary.Messages.Authenticator.RoleManagement;

namespace DAPM.Authenticator.Consumers
{
    public class GetRolesMessageConsumer : IQueueConsumer<GetRolesMessage>
    {
        public Task ConsumeAsync(GetRolesMessage message)
        {
            return Task.CompletedTask;
        }
    }
}

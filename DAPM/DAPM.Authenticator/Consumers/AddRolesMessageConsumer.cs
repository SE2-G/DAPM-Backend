using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using RabbitMQLibrary.Interfaces;
using RabbitMQLibrary.Models;
using RabbitMQLibrary.Messages.Authenticator.RoleManagement;

namespace DAPM.Authenticator.Consumers
{
    public class AddRolesMessageConsumer : IQueueConsumer<AddRolesMessage>
    {
        public Task ConsumeAsync(AddRolesMessage message)
        {
            return Task.CompletedTask;
        }
    }
}

using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using RabbitMQLibrary.Interfaces;
using RabbitMQLibrary.Models;
using RabbitMQLibrary.Messages.Authenticator.UserManagement;

namespace DAPM.Authenticator.Consumers
{
    public class SetRolesMessageConsumer : IQueueConsumer<SetRolesMessage>
    {
        public Task ConsumeAsync(SetRolesMessage message)
        {
            return Task.CompletedTask;
        }
    }
}

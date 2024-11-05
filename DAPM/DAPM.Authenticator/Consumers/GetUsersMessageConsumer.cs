using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using RabbitMQLibrary.Interfaces;
using RabbitMQLibrary.Models;
using RabbitMQLibrary.Messages.Authenticator.UserManagement;

namespace DAPM.Authenticator.Consumers
{
    public class GetUsersMessageConsumer : IQueueConsumer<GetUsersMessage>
    {
        public Task ConsumeAsync(GetUsersMessage message)
        {
            return Task.CompletedTask;
        }
    }
}

// Author: s224755

using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using RabbitMQLibrary.Interfaces;
using RabbitMQLibrary.Models;
using RabbitMQLibrary.Messages.Authenticator.RoleManagement;
using DAPM.Authenticator.Models;
using Microsoft.AspNetCore.Identity;
using RabbitMQLibrary.Messages.Authenticator.Base;
using DAPM.Authenticator.Interfaces;

namespace DAPM.Authenticator.Consumers
{
    public class DeleteRolesMessageConsumer : IQueueConsumer<DeleteRolesMessage>
    {
        private readonly IQueueProducer<DeleteRolesResultMessage> _deleteRolesResultProducer;
        private readonly IRoleManagerWrapper _rolemanager;

        public DeleteRolesMessageConsumer(IQueueProducer<DeleteRolesResultMessage> deleteRolesResultProducer, IRoleManagerWrapper rolemanager)
        {
            _deleteRolesResultProducer = deleteRolesResultProducer;
            _rolemanager = rolemanager;
        }

        public async Task ConsumeAsync(DeleteRolesMessage message)
        {
            try
            {
                string result = "";
                
                foreach (var role in message.Roles)
                {
                    if(role == "Admin" || role == "Standard")
                    {
                        result += role + " role cannot be deleted.\n";
                    }
                    else if (await _rolemanager.RoleExistsAsync(role))
                    {
                        try
                        {
                            var r = await _rolemanager.FindByNameAsync(role);
                            
                            await _rolemanager.DeleteAsync(r);

                            result += "Deleted role \"" + role + "\".\n";
                        }
                        catch (Exception ex)
                        {
                            result += "Failed to delete role \"" + role + "\":" + ex.Message + "\n";
                        }
                    }
                    else
                    {
                        result += "Role \"" + role + "\" does not exist.\n";
                    }
                }

                var deleteRolesResultMessage = new DeleteRolesResultMessage
                {
                    TimeToLive = TimeSpan.FromMinutes(1),
                    TicketId = message.TicketId,
                    Succeeded = true,
                    Message = result
                };

                _deleteRolesResultProducer.PublishMessage(deleteRolesResultMessage);
            }
            catch (Exception ex)
            {
                var deleteRolesResultMessage = new DeleteRolesResultMessage
                {
                    TimeToLive = TimeSpan.FromMinutes(1),
                    TicketId = message.TicketId,
                    Succeeded = false,
                    Message = ex.Message
                };

                _deleteRolesResultProducer.PublishMessage(deleteRolesResultMessage);
            }
        }
    }
}

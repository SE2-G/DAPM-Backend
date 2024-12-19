// Author: s205135

using DAPM.Authenticator.Consumers;
using DAPM.Authenticator.Interfaces;
using DAPM.Authenticator.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Identity.Client;
using Moq;
using RabbitMQLibrary.Interfaces;
using RabbitMQLibrary.Messages.Authenticator.Base;
using RabbitMQLibrary.Messages.Authenticator.RoleManagement;
using RabbitMQLibrary.Messages.ClientApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unit_tests
{
    public class DeleteRolesMessageConsumerTest
    {
        DeleteRolesMessageConsumer consumer;
        List<Role> roles;
        DeleteRolesMessage _roleRemoveRequest = new DeleteRolesMessage();
        DeleteRolesMessage _roleRemoveRequest2 = new DeleteRolesMessage();
        DeleteRolesMessage _roleRemoveRequest3 = new DeleteRolesMessage();
        DeleteRolesMessage _roleRemoveRequest4 = new DeleteRolesMessage();
        DeleteRolesResultMessage result;

        [SetUp]
        public void Setup() {
            roles = new List<Role>() { new Role() { Id = 1, Name = "Admin" }, new Role() { Id = 1, Name = "Standard" }, new Role() { Id = 1, Name = "Priviliged" }, new Role() { Id = 1, Name = "Peasant" } };
            Mock<IQueueProducer<DeleteRolesResultMessage>> _mockqueueu = new Mock<IQueueProducer<DeleteRolesResultMessage>>();
            _mockqueueu.Setup(queue => queue.PublishMessage(It.IsAny<DeleteRolesResultMessage>()))
                            .Callback<DeleteRolesResultMessage>(v => result = v);
  
                    


            Mock<IRoleManagerWrapper> _mockrolemanager = new Mock<IRoleManagerWrapper>();

            _mockrolemanager.Setup(repo => repo.RoleExistsAsync(It.IsAny<string>())).ReturnsAsync((string role) => {
                return  roles.FirstOrDefault(x => x.Name.Equals(role, StringComparison.OrdinalIgnoreCase))  == null ? false : true;
            });

            _mockrolemanager.Setup(repo => repo.DeleteAsync(It.IsAny<Role>())).ReturnsAsync((Role role) =>
            {
                roles = roles.Where(x => x.Name != role.Name).ToList();
                return IdentityResult.Success;
            });


            _mockrolemanager.Setup(repo => repo.FindByNameAsync(It.IsAny<string>()))
                            .ReturnsAsync((string name) => {
                                return roles.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
                            });


            consumer = new DeleteRolesMessageConsumer(
                _mockqueueu.Object,
                _mockrolemanager.Object);


            _roleRemoveRequest = new DeleteRolesMessage
            {
                MessageId = Guid.NewGuid(),
                TicketId = Guid.NewGuid(),
                TimeToLive = TimeSpan.FromMinutes(1),
                Roles = new List<string>() { "Admin" }
            };
            _roleRemoveRequest2 = new DeleteRolesMessage
            {
                MessageId = Guid.NewGuid(),
                TicketId = Guid.NewGuid(),
                TimeToLive = TimeSpan.FromMinutes(1),
                Roles = new List<string>() { "Priviliged", "Peasant" }
            };
            _roleRemoveRequest3 = new DeleteRolesMessage
            {
                MessageId = Guid.NewGuid(),
                TicketId = Guid.NewGuid(),
                TimeToLive = TimeSpan.FromMinutes(1),
                Roles = new List<string>() { "farmer" }
            };
            _roleRemoveRequest4 = new DeleteRolesMessage
            {
                MessageId = Guid.NewGuid(),
                TicketId = Guid.NewGuid(),
                TimeToLive = TimeSpan.FromMinutes(1),
                Roles = new List<string>() { "Priviliged"}
            };

        }


        [Test]
        public async Task TestRemovalOfAdminRole() {
            Assert.True(roles.Count == 4);
            await consumer.ConsumeAsync(_roleRemoveRequest);
            Assert.True(roles.Count == 4);
            Assert.True(result.Message == "Admin role cannot be deleted.\n");
        }

        [Test]
        public async Task TestRemovalOfSingleNormalRole()
        {
            Assert.True(roles.Count == 4);
            await consumer.ConsumeAsync(_roleRemoveRequest4);
            Assert.True(roles.Count == (4 - _roleRemoveRequest4.Roles.Count));
            Assert.True(result.Succeeded == true);
        }

        [Test]
        public async Task TestRemovalOfMultipleNormalRole()
        {
            Assert.True(roles.Count == 4);
            await consumer.ConsumeAsync(_roleRemoveRequest2);
            Assert.True(roles.Count == (4 - _roleRemoveRequest2.Roles.Count));
            Assert.True(result.Succeeded == true);
        }

        [Test]
        public async Task TestRemovalOfRoleThatDoesNotExist()
        {
            Assert.True(roles.Count == 4);
            await consumer.ConsumeAsync(_roleRemoveRequest3);
            Assert.True(roles.Count == 4);
            Assert.True(result.Message == "Role \"" + _roleRemoveRequest3.Roles[0] + "\" does not exist.\n");
            Assert.True(result.Succeeded == true);
        }



    }
}

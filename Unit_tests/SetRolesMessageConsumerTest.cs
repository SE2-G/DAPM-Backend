using DAPM.Authenticator.Consumers;
using DAPM.Authenticator.Interfaces;
using DAPM.Authenticator.Interfaces.Repostory_Interfaces;
using DAPM.Authenticator.Models;
using Moq;
using RabbitMQLibrary.Interfaces;
using RabbitMQLibrary.Messages.Authenticator.Base;
using RabbitMQLibrary.Messages.Authenticator.UserManagement;
using RabbitMQLibrary.Messages.ClientApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unit_tests
{
    public class SetRolesMessageConsumerTest
    {
        SetRolesMessageConsumer consumer;
        SetRolesMessage message = new SetRolesMessage()
        {

            MessageId = Guid.NewGuid(),
            TicketId = Guid.NewGuid(),
            TimeToLive = TimeSpan.FromSeconds(1),
            UserName = "johnny",
            Roles = new List<string>() { "Admin"}
        };

        List<(User, List<Role>)> usersandRoles = new List<(User, List<Role>)> {
             (
                new User {
                FullName = "Jimbob",
                PasswordHash = "",
                Id = 5,
                UserName = "johnny" },
                new List<Role>(){new Role{Name= "Standard"} })};

        List<string> roles = new List<string>() { "Admin", "Standard", "Priviliged" };



        [SetUp]
        public void Setup() {
            Mock<IQueueProducer<SetRolesResultMessage>> _mockqueueu = new Mock<IQueueProducer<SetRolesResultMessage>>();
            _mockqueueu.Setup(q => q.PublishMessage(It.IsAny<SetRolesResultMessage>()));

            Mock<IUserManagerWrapper> _mockusermanager = new Mock<IUserManagerWrapper>();
            _mockusermanager.Setup(usermanager => usermanager.FindByNameAsync(It.IsAny<string>()))
                .Returns((string input) => Task.FromResult(usersandRoles.FirstOrDefault(x => x.Item1.UserName.Equals(input, StringComparison.OrdinalIgnoreCase)).Item1));


            Mock<IRoleManagerWrapper> _mockrolemanager = new Mock<IRoleManagerWrapper>();
            _mockrolemanager.Setup(repo => repo.RoleExistsAsync(It.IsAny<string>())).ReturnsAsync((string role) => roles.Contains(role));

            Mock<IUserRepository> _mockuserrepo = new Mock<IUserRepository>();
            _mockuserrepo.Setup(repo => repo.SaveChanges(It.IsAny<User>()));

            consumer = new SetRolesMessageConsumer(
                _mockusermanager.Object,
                _mockrolemanager.Object,
                _mockuserrepo.Object,
                _mockqueueu.Object
                );
        }

        [Test]
        public async Task SetRolesTest() { 
        
            await consumer.ConsumeAsync( message);
        
        }

    }
}

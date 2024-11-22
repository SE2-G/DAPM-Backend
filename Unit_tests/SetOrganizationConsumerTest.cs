using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAPM.Authenticator.Consumers;
using DAPM.Authenticator.Interfaces;
using DAPM.Authenticator.Interfaces.Repostory_Interfaces;
using DAPM.Authenticator.Models;
using Moq;
using RabbitMQLibrary.Interfaces;
using RabbitMQLibrary.Messages.Authenticator.Base;
using RabbitMQLibrary.Messages.Authenticator.UserManagement;
using RabbitMQLibrary.Messages.ClientApi;

namespace Unit_tests
{
    public class SetOrganizationConsumerTest
    {

        SetOrganizationMessageConsumer consumer;


        List<User> users= new List<User> {
             
                new User {
                FullName = "Jimbob",
                PasswordHash = "",
                Id = 5,
                UserName = "johnny" }};

        SetOrganizationMessage message = new SetOrganizationMessage() {

            MessageId = Guid.NewGuid(),
            TicketId = Guid.NewGuid(),
            TimeToLive = TimeSpan.FromSeconds(1),
            UserName = "Jimbob",
            OrganizationId = Guid.NewGuid(),
            OrganizationName = "SDU"
        };

        [SetUp]
        public void Setup() {

            Mock<IQueueProducer<SetOrganizationResultMessage>> _mockqueueu = new Mock<IQueueProducer<SetOrganizationResultMessage>>();
            _mockqueueu.Setup(q => q.PublishMessage(It.IsAny<SetOrganizationResultMessage>()));



            Mock<IUserManagerWrapper> _mockusermanager = new Mock<IUserManagerWrapper>();
            _mockusermanager.Setup(usermanager => usermanager.FindByNameAsync(It.IsAny<string>()))
                .Returns((string input) => Task.FromResult(users.FirstOrDefault(x => x.UserName.Equals(input, StringComparison.OrdinalIgnoreCase))));


            Mock<IUserRepository> _mockuserrepo = new Mock<IUserRepository>();
            _mockuserrepo.Setup(repo => repo.SaveChanges(It.IsAny<User>()));


            consumer = new SetOrganizationMessageConsumer(
                    _mockusermanager.Object,
                    _mockuserrepo.Object,
                    _mockqueueu.Object );
        }

        [Test]
        public async Task SetOrgTest() {

            await consumer.ConsumeAsync(message);
        
        }
    }
}

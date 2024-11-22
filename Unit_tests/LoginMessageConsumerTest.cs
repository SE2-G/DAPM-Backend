using AutoMapper;
using Castle.Core.Logging;
using DAPM.Authenticator.Consumers;
using DAPM.Authenticator.Interfaces;
using DAPM.Authenticator.Interfaces.Repostory_Interfaces;
using DAPM.Authenticator.Models;
using Microsoft.Extensions.Logging;
using Moq;
using RabbitMQLibrary.Interfaces;
using RabbitMQLibrary.Messages.Authenticator.Base;
using RabbitMQLibrary.Messages.ClientApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibrary;

namespace Unit_tests
{
    public class LoginMessageConsumerTest
    {
        LoginMessageConsumer consumer;
        LoginMessage message = new LoginMessage() {

            MessageId = Guid.NewGuid(),
            TicketId = Guid.NewGuid(),
            TimeToLive = TimeSpan.FromSeconds(1),
            UserName = "Jimbob",
            Password = "passW1",
        };

        List<(User, List<Role>, string)> usersRolesAndPasswords = new List<(User, List<Role>, string)> {
             (
                new User {
                FullName = "Jimbob",
                PasswordHash = "",
                Id = 5,
                UserName = "johnny" },
                new List<Role>(){new Role{Name= "Standard"} },
                "passW1")};

        [SetUp]
        public void Setup() {


            Mock<IQueueProducer<LoginResultMessage>> _mockqueueu = new Mock<IQueueProducer<LoginResultMessage>>();
            _mockqueueu.Setup(q => q.PublishMessage(It.IsAny<LoginResultMessage>()));

            Mock<ILogger<LoginMessageConsumer>> _mocklogger = new Mock<ILogger<LoginMessageConsumer>>();

            Mock<IMapper> _mockmapper = new Mock<IMapper>();
            _mockmapper.Setup(m => m.Map<UserDto>(It.IsAny<User>())).Returns((User user) => {
                return new UserDto()
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    UserName = user.UserName,
                    OrganizationId = user.OrganizationId,
                    OrganizationName = user.OrganizationName,
                };
            });

            Mock<IUserManagerWrapper> _mockusermanager = new Mock<IUserManagerWrapper>();
            _mockusermanager.Setup(usermanager => usermanager.FindByNameAsync(It.IsAny<string>()))
                .Returns((string input) => Task.FromResult(usersRolesAndPasswords.FirstOrDefault(x => x.Item1.UserName.Equals(input, StringComparison.OrdinalIgnoreCase)).Item1));

            _mockusermanager.Setup(usermanager => usermanager.CheckPasswordAsync(It.IsAny<User>(), It.IsAny<string>()))
                            .ReturnsAsync((User user, string password) => {

                                (User, List<Role>, string) retrieval = usersRolesAndPasswords.FirstOrDefault(x => x.Item1.UserName.Equals(user.UserName, StringComparison.OrdinalIgnoreCase));
                                if (retrieval.Item1 == null) {
                                    return false;
                                }

                                return password == retrieval.Item3;                                                 
                            });


            Mock<IUserRepository> _mockuserrepo = new Mock<IUserRepository>();
            _mockuserrepo.Setup(repo => repo.SaveChanges(It.IsAny<User>()));


            Mock<ITokenService> _mockTokenService = new Mock<ITokenService>();
            _mockTokenService.Setup(t => t.CreateToken(It.IsAny<User>())).ReturnsAsync("blalblatokenconetns");


            consumer = new LoginMessageConsumer(
                    _mocklogger.Object,
                    _mockmapper.Object,
                    null,
                    _mockusermanager.Object,
                    _mockuserrepo.Object,
                    null,
                    _mockTokenService.Object,
                    _mockqueueu.Object
                );
        }

        [Test]
        public async Task TestLogin() {

            await consumer.ConsumeAsync(message);
        
        }



    }
}

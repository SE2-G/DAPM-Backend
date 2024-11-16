using AutoMapper;
using DAPM.Authenticator.Consumers;
using DAPM.Authenticator.Interfaces;
using DAPM.Authenticator.Interfaces.Repostory_Interfaces;
using DAPM.Authenticator.Models;
using DAPM.Authenticator.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using Moq;
using RabbitMQLibrary.Interfaces;
using RabbitMQLibrary.Messages.Authenticator.Base;
using RabbitMQLibrary.Messages.ClientApi;
using UtilLibrary.Interfaces;

namespace Unit_tests
{
    public class Tests
    {
        RegisterUserMessageConsumer registerconsumer;

        RegisterUserMessage source;
        User dest;

        RegisterUserResultMessage result;

        Guid ticketId = Guid.NewGuid();

        [SetUp]
        public void Setup()
        {
            Mock<IMapper> _mockMapper = new Mock<IMapper>();

            source = new RegisterUserMessage
            {
                TimeToLive = TimeSpan.FromMinutes(1),
                TicketId = ticketId,
                FullName = "Test McTestersson",
                Password = "passworD123",
                UserName = "Test",
                OrganizationId = Guid.Parse("d87bc490-828f-46c8-aa44-ded7729eaa82"),
                OrganizationName = "DTU",
                Roles = new List<string> { "Standard" }
            };

            dest = new User()
            {
                FullName = "Test McTestersson",
                UserName = "Test",
                OrganizationId = Guid.NewGuid(),
                OrganizationName = "Test",
            };

            result = new RegisterUserResultMessage
            {
                TimeToLive = TimeSpan.FromMinutes(1),
                TicketId = ticketId,
                Message = "",
                Succeeded = true
            };

            _mockMapper.Setup(mapper => mapper.Map<User>(source)).Returns(dest);


            Mock<IUserRepository> _mockUserRepo = new Mock<IUserRepository>();

            _mockUserRepo.Setup(repo => repo.UserExists(It.IsAny<string>())).Returns(true);
            _mockUserRepo.Setup(repo => repo.SaveChanges(It.IsAny<User>())).Returns(1);


            Mock<IUserManagerWrapper> _mockusermanager = new Mock<IUserManagerWrapper>();
            //TODO setup for usermanager
            _mockusermanager.Setup(userman => userman.CreateAsync(It.IsAny<User>(), It.IsAny<string>())).Returns(Task.FromResult(IdentityResult.Success));
            _mockusermanager.Setup(userman => userman.AddToRoleAsync(It.IsAny<User>(), It.IsAny<string>())).Returns(Task.FromResult(IdentityResult.Success));


            IConfiguration configuration = new ConfigurationBuilder()
                                            .SetBasePath(Directory.GetCurrentDirectory())
                                            .AddJsonFile("appsettings.json").Build();

            Mock <IIdentityService> _mockidentityService = new Mock<IIdentityService>();
            _mockidentityService.Setup(iservice => iservice.GetIdentity()).Returns(new UtilLibrary.models.Identity {
                Id = Guid.Parse("d87bc490-828f-46c8-aa44-ded7729eaa82"),
                Name = "DTU",
                Domain = "dapm1.compute.dtu.dk"
            });

            Mock<IQueueProducer<RegisterUserResultMessage>> _mockqueueu = new Mock<IQueueProducer<RegisterUserResultMessage>>();
            _mockqueueu.Setup(queue => queue.PublishMessage(It.IsAny<RegisterUserResultMessage>()));



            registerconsumer = new RegisterUserMessageConsumer(
                     null,
                     _mockMapper.Object,
                     null,
                     _mockusermanager.Object,
                     _mockUserRepo.Object,
                     null,
                     null,
                     _mockidentityService.Object,
                     _mockqueueu.Object
              );
        }

        [Test]
        public async Task Test1()
        {
            await registerconsumer.ConsumeAsync(source);

        }
    }
}
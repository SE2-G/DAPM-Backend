using DAPM.Authenticator.Consumers;
using DAPM.Authenticator.Interfaces;
using DAPM.Authenticator.Interfaces.Repostory_Interfaces;
using DAPM.Authenticator.Models;
using Microsoft.AspNetCore.Identity;
using Moq;
using RabbitMQLibrary.Interfaces;
using RabbitMQLibrary.Messages.Authenticator.Base;
using RabbitMQLibrary.Messages.Authenticator.UserManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibrary.models;

namespace Unit_tests
{
    public class EditAsAdminMessageConsumerTest
    {

        List<string> allroles = new List<string>() { "Standard", "Admin", "Privileged" };


        EditAsAdminMessageConsumer consumer;
        EditAsAdminMessage message = new EditAsAdminMessage {
            MessageId = Guid.NewGuid(),
            TicketId = Guid.NewGuid(),
            TimeToLive = TimeSpan.FromSeconds(1),
            Id = 5,
            FullName = "Jimbob",
            UserName = "bobby",
            NewPassword = "",
            Roles = new List<string> { "Admin", "Standard" }
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

            Mock<IQueueProducer<EditAsAdminResultMessage>> _mockqueueu = new Mock<IQueueProducer<EditAsAdminResultMessage>>();
            _mockqueueu.Setup(queue => queue.PublishMessage(It.IsAny<EditAsAdminResultMessage>()));

            Mock<IUserManagerWrapper> _mockusermanager = new Mock<IUserManagerWrapper>();
            _mockusermanager.Setup(usermanager => usermanager.FindByIdAsync(It.IsAny<string>()))
                .Returns((string input) => Task.FromResult(usersRolesAndPasswords.FirstOrDefault(x => x.Item1.Id == int.Parse(input)).Item1));

            _mockusermanager.Setup(usermanager => usermanager.GetRolesAsync(It.IsAny<User>())).ReturnsAsync(
                (User input) => {
                    (User, List<Role>, string) user = usersRolesAndPasswords.FirstOrDefault(x => x.Item1.Id == input.Id);
                    return user.Item1 != null ? user.Item2.Select(x => x.Name).ToList() : new List<string>();              
                }    
            );

            _mockusermanager.Setup(usermanager => usermanager.RemoveFromRoleAsync(It.IsAny<User>(), It.IsAny<string>())).Returns(
                (User userinput, string roleinput) => {
                    (User, List<Role>, string) user = usersRolesAndPasswords.FirstOrDefault(x => x.Item1.Id == userinput.Id);
                    user.Item2 = user.Item2.Where(x => x.Name != roleinput).ToList();
                    return Task.FromResult(IdentityResult.Success);
                }
              );

            _mockusermanager.Setup(usermanager => usermanager.RemovePasswordAsync(It.IsAny<User>())).Returns(
                (User userinput) => {
                    (User, List<Role>, string) user = usersRolesAndPasswords.FirstOrDefault(x => x.Item1.Id == userinput.Id);
                    user.Item3 = "";
                    return Task.FromResult(IdentityResult.Success);
                }
              );

            _mockusermanager.Setup(usermanager => usermanager.AddPasswordAsync(It.IsAny<User>(), It.IsAny<string>())).Returns(
                (User userinput, string pass) => {
                    (User, List<Role>, string) user = usersRolesAndPasswords.FirstOrDefault(x => x.Item1.Id == userinput.Id);
                    user.Item3 = pass;
                    return Task.FromResult(IdentityResult.Success);
                }
              );

            Mock<IUserRepository> _mockuserrepo = new Mock<IUserRepository>();
            _mockuserrepo.Setup(repo => repo.SaveChanges(It.IsAny<User>()));

            Mock<IRoleManagerWrapper> _mockrolemanager = new Mock<IRoleManagerWrapper>();
            _mockrolemanager.Setup(repo => repo.RoleExistsAsync(It.IsAny<string>())).ReturnsAsync((string role) => allroles.Contains(role));



            consumer = new EditAsAdminMessageConsumer(
                _mockusermanager.Object,
                _mockrolemanager.Object,
                _mockuserrepo.Object,
                _mockqueueu.Object);



        }

        [Test]
        public async Task EditUserTestAsAdmin() {
            await consumer.ConsumeAsync(message);     
        }

    }
}

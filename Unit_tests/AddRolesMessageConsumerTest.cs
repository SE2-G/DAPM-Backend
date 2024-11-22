﻿using DAPM.Authenticator.Consumers;
using DAPM.Authenticator.Interfaces;
using DAPM.Authenticator.Models;
using Microsoft.AspNetCore.Identity;
using Moq;
using RabbitMQLibrary.Interfaces;
using RabbitMQLibrary.Messages.Authenticator.Base;
using RabbitMQLibrary.Messages.Authenticator.RoleManagement;
using RabbitMQLibrary.Messages.ClientApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unit_tests
{
    public class AddRolesMessageConsumerTest
    {
        AddRolesMessageConsumer consumer;
        List<Role> roles = new List<Role>();
        AddRolesMessage _roleAddRequest = new AddRolesMessage();


       [SetUp]
        public void Setup() {

            Mock<IQueueProducer<AddRolesResultMessage>> _mockqueueu = new Mock<IQueueProducer<AddRolesResultMessage>>();
            _mockqueueu.Setup(queue => queue.PublishMessage(It.IsAny<AddRolesResultMessage>()));

            Mock<IRoleManagerWrapper> _mockrolemanager = new Mock<IRoleManagerWrapper>();
            _mockrolemanager.Setup(man => man.CreateAsync(It.IsAny<Role>()))
                            .Returns(Task.FromResult(IdentityResult.Success))
                            .Callback<Role>( v => roles.Add(v));


            consumer = new AddRolesMessageConsumer(
                _mockqueueu.Object,
                _mockrolemanager.Object);


            _roleAddRequest = new AddRolesMessage {
                MessageId = Guid.NewGuid(),
                TicketId = Guid.NewGuid(),
                TimeToLive = TimeSpan.FromMinutes(1),
                Roles = new List<string>() {"Admin"}
            }; 
        }

        [Test]
        public async Task AddOneRole()
        {
            Assert.True( roles.Count == 0);
            await consumer.ConsumeAsync(_roleAddRequest);
            Assert.True(roles.Count == _roleAddRequest.Roles.Count);

        }

           
    }
}

﻿using RabbitMQLibrary.Interfaces;
using RabbitMQLibrary.Models;

namespace RabbitMQLibrary.Messages.Authenticator.Base
{
    public class SetRolesResultMessage : IQueueMessage
    {
        public Guid MessageId { get; set; }
        public TimeSpan TimeToLive { get; set; }

        public bool Succeeded { get; set; }
        public string Message { get; set; }
    }
}

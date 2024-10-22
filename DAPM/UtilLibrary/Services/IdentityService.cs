﻿using Microsoft.Extensions.DependencyInjection;
//using RabbitMQLibrary.Interfaces;
//using RabbitMQLibrary.Messages.Repository;
//using RabbitMQLibrary.Messages.ResourceRegistry;
//using RabbitMQLibrary.Models;
using System.Reflection;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using UtilLibrary.models;
using UtilLibrary.Interfaces;

namespace UtilLibrary.Services
{
    public class IdentityService : IIdentityService
    {
        private ILogger<IdentityService> _logger;
        private string _identityConfigurationPath;
        private IServiceProvider _serviceProvider;
        private IServiceScope _serviceScope;

        public IdentityService(ILogger<IdentityService> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _serviceScope = _serviceProvider.CreateScope();
            var currentDirectory = Directory.GetCurrentDirectory();
            var path = Path.Combine(Directory.GetCurrentDirectory(), "Services");
            path = Path.Combine(path, "Configuration");
            _identityConfigurationPath = Path.Combine(path, "IdentityConfiguration.json");
        }

        public Identity GenerateNewIdentity()
        {

            Identity identity = ReadCurrentIdentity();

            identity.Id = Guid.NewGuid();

            string jsonString = JsonSerializer.Serialize(identity);
            File.WriteAllText(_identityConfigurationPath, jsonString);

            //PostIdentityToRegistry(identity);

            return identity;
        }

        public Identity? GetIdentity()
        {
            Identity identity = ReadCurrentIdentity();

            if (identity.Id == null || !identity.Id.HasValue)
            {
                return GenerateNewIdentity();
            }
            else
            {
                //PostIdentityToRegistry(identity);
                return identity;
            }
        }


        private Identity ReadCurrentIdentity()
        {
            var resourceNames = typeof(CustomTokenTypeConstants).Assembly.GetManifestResourceNames();//to access the assembly of utillibarary
            using (Stream stream = typeof(CustomTokenTypeConstants).Assembly.GetManifestResourceStream("UtilLibrary.Configuration.IdentityConfiguration.json"))
            {
                if (stream == null)
                {
                    throw new FileNotFoundException("Identity Resource not found");
                }
                else
                {

                    using (StreamReader reader = new StreamReader(stream))
                    {
                        string jsonString = reader.ReadToEnd();
                        return JsonSerializer.Deserialize<Identity>(jsonString)!;
                    }
                }
            }
        }

        /*
        private void PostIdentityToRegistry(Identity identity)
        {
            var postPeerProducer = _serviceScope.ServiceProvider.GetRequiredService<IQueueProducer<PostPeerMessage>>();

            var postPeerMessage = new PostPeerMessage()
            {
                TimeToLive = TimeSpan.FromMinutes(1),
                Organization = new OrganizationDTO()
                {
                    Id = (Guid)identity.Id,
                    Name = identity.Name,
                    Domain = identity.Domain
                }
            };

            postPeerProducer.PublishMessage(postPeerMessage);
        }
        */
    }
}
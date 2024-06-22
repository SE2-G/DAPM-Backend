﻿using DAPM.Orchestrator.Processes;
using RabbitMQLibrary.Interfaces;
using RabbitMQLibrary.Messages.Orchestrator.ServiceResults.FromRegistry;

namespace DAPM.Orchestrator.Consumers.ResultConsumers.FromRegistry
{
    public class GetOrgsFromRegistryResultConsumer : IQueueConsumer<GetOrganizationsResultMessage>
    {
        private IOrchestratorEngine _orchestratorEngine;

        public GetOrgsFromRegistryResultConsumer(IOrchestratorEngine orchestratorEngine)
        {
            _orchestratorEngine = orchestratorEngine;
        }

        public Task ConsumeAsync(GetOrganizationsResultMessage message)
        {
            GetOrganizationsProcess process = (GetOrganizationsProcess)_orchestratorEngine.GetProcess(message.TicketId);
            process.OnGetOrganizationsFromRegistryResult(message);

            return Task.CompletedTask;
        }
    }
}
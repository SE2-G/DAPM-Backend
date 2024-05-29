using RabbitMQLibrary.Implementation;
using RabbitMQLibrary.Extensions;
using RabbitMQLibrary.Messages.ResourceRegistry;
using DAPM.Orchestrator.Consumers.StartProcessConsumers;
using RabbitMQLibrary.Messages.Orchestrator.ProcessRequests;
using DAPM.Orchestrator.Consumers.ResultConsumers;
using DAPM.Orchestrator;
using RabbitMQLibrary.Messages.Orchestrator.ServiceResults;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddQueueing(new QueueingConfigurationSettings
{
    RabbitMqConsumerConcurrency = 5,
    RabbitMqHostname = "rabbitmq",
    RabbitMqPort = 5672,
    RabbitMqPassword = "guest",
    RabbitMqUsername = "guest"
});

//StartRequests
builder.Services.AddQueueMessageConsumer<GetOrganizationsRequestConsumer, GetOrganizationsRequest>();
builder.Services.AddQueueMessageConsumer<GetRepositoriesRequestConsumer, GetRepositoriesRequest>();
builder.Services.AddQueueMessageConsumer<GetResourcesRequestConsumer, GetResourcesRequest>();
builder.Services.AddQueueMessageConsumer<PostResourceRequestConsumer, PostResourceRequest>();




//ServicesResults
builder.Services.AddQueueMessageConsumer<GetOrgsFromRegistryResultConsumer, GetOrganizationsResultMessage>();
builder.Services.AddQueueMessageConsumer<GetReposFromRegistryResultConsumer, GetRepositoriesResultMessage>();
builder.Services.AddQueueMessageConsumer<GetResourcesFromRegistryResultConsumer, GetResourcesResultMessage>();
builder.Services.AddQueueMessageConsumer<PostResourceToRepoResultConsumer, PostResourceToRepoResultMessage>();
builder.Services.AddQueueMessageConsumer<PostResourceToRegistryResultConsumer, PostResourceToRegistryResultMessage>();




builder.Services.AddSingleton<IOrchestratorEngine, OrchestratorEngine>();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
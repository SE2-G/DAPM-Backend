using DAPM.ClientApi.Services.Interfaces;
using RabbitMQLibrary.Interfaces;
using RabbitMQLibrary.Messages.Authenticator.Base;
using RabbitMQLibrary.Messages.ClientApi;
using RabbitMQLibrary.Messages.Orchestrator.ProcessRequests;
using UtilLibrary;

namespace DAPM.ClientApi.Services
{
    public class AuthenticatorService : IAuthenticatorService
    {
        private readonly ILogger<AuthenticatorService> _logger;
        private readonly ITicketService _ticketService;
        private readonly IQueueProducer<RegisterUserMessage> _registerUserMessageProducer;
        private readonly IQueueProducer<LoginMessage> _loginMessageProducer;

        public AuthenticatorService(
            ILogger<AuthenticatorService> logger,
            ITicketService ticketService,
            IQueueProducer<RegisterUserMessage> registerUserMessageProducer,
            IQueueProducer<LoginMessage> loginMessageProducer
        ) {
            _logger = logger;
            _ticketService = ticketService; 
            _registerUserMessageProducer = registerUserMessageProducer;
            _loginMessageProducer = loginMessageProducer;
        }


        public Guid RegisterUser(RegistrationDto registerDto)
        {
            Guid ticketId = _ticketService.CreateNewTicket(TicketResolutionType.Json);

            var message = new RegisterUserMessage
            {
                TimeToLive = TimeSpan.FromMinutes(1),
                TicketId = ticketId,
                FullName = registerDto.FullName,
                Password = registerDto.Password,
                UserName = registerDto.UserName,
                OrganizationId = registerDto.OrganizationId,
                OrganizationName = registerDto.OrganizationName
            };

            _registerUserMessageProducer.PublishMessage(message);
            _logger.LogDebug("RegisterUserMessage Enqueued");

            return ticketId;
        }

        public Guid Login(LoginDto loginDto)
        {
            Guid ticketId = _ticketService.CreateNewTicket(TicketResolutionType.Json);

            var message = new LoginMessage
            {
                TimeToLive = TimeSpan.FromMinutes(1),
                TicketId = ticketId,
                Password = loginDto.Password,
                UserName = loginDto.UserName
            };

            _loginMessageProducer.PublishMessage(message);
            _logger.LogDebug("LoginMessage Enqueued");

            return ticketId;
        }

        public void RemoveUser()
        {
            
        }

        public void SignUp()
        {
        
        }
    }
}

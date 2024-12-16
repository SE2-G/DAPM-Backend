using DAPM.PeerApi.Models.PeerUserDtos;
using DAPM.PeerApi.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RabbitMQLibrary.Interfaces;
using RabbitMQLibrary.Messages.Orchestrator.ServiceResults.FromPeerApi;
using UtilLibrary;
using UtilLibrary.Interfaces;
using UtilLibrary.models;


namespace DAPM.PeerApi.Controllers
{
    [ApiController]
    [Route("user/")]
    public class PeerUserController : ControllerBase
    {
        private readonly ILogger<RegistryController> _logger;
        private IPeerUserService _peerUserService;

        public PeerUserController(ILogger<RegistryController> logger,
            IQueueProducer<RegistryUpdateMessage> registryUpdateProducer,
            IPeerUserService peerUserService)
        {
            _logger = logger;
            _peerUserService = peerUserService;
        }

        [HttpPost("authenticate")]
        public async Task<ActionResult<Guid>> Authenticate([FromBody] AuthenticationDto authDto)
        {
            if (authDto == null)
            {
                return BadRequest("payload empty");
            }

            _peerUserService.OnAuthenticateRequest(authDto.AuthenticationId, authDto.SenderIdentity, authDto.UserName, authDto.Password);

            return Ok("Authenticate request received");
        }

        [HttpPost("authenticate-respone")]
        public async Task<ActionResult<Guid>> AuthenticateResponse([FromBody] AuthenticationResponseDto authDto)
        {
            if (authDto == null)
            {
                return BadRequest("payload empty");
            }

            _peerUserService.OnAuthenticateRequestResponse(authDto.AuthenticationId, authDto.SenderIdentity, authDto.SessionToken, authDto.IsAuthenticated);

            return Ok("Authenticate request received");
        }
    }
}

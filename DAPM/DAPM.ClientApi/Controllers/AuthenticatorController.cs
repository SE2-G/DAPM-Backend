using DAPM.ClientApi.Models;
using DAPM.ClientApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.ObjectPool;
using Newtonsoft.Json;
using RabbitMQLibrary.Messages.Authenticator.Base;
using System.Text;
using UtilLibrary;
using UtilLibrary.Interfaces;
using UtilLibrary.models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DAPM.ClientApi.Controllers
{

    [ApiController]
    [EnableCors("AllowAll")]
    [Route("auth")]
    public class AuthenticatorController : BaseController
    {
        private readonly ILogger<AuthenticatorController> _logger;
        private readonly HttpClient _httpClient;
        private readonly IIdentityService _identityService;
        private readonly IConfiguration _configuration;
        private readonly IAuthenticatorService _authenticatorService;

        //private IAuthenticatorService _authenticationService;

        public AuthenticatorController(
            ILogger<AuthenticatorController> logger, 
            IHttpContextAccessor contextAccessor,
            HttpClient httpClient,
            IAuthenticatorService authenticatorService,
            IIdentityService identityService,
            IConfiguration configuration): base(contextAccessor)
        {
            _logger = logger;
            _httpClient = httpClient;
            _identityService = identityService;
            _configuration = configuration;
            _authenticatorService = authenticatorService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<Guid>> Register([FromBody] RegistrationDto registerDto)
        {

            Identity identity = _identityService.GetIdentity();

            if (identity == null) {
                return StatusCode(500, "Failed to retrieve peer identity");
            }

            registerDto.OrganizationId = identity.Id.Value;
            registerDto.OrganizationName = identity.Name;

            Guid id = _authenticatorService.RegisterUser(registerDto);
            return Ok(new ApiResponse { RequestName = "RegisterUser", TicketId = id });
        }

        [HttpPost("login")]
        public async Task<ActionResult<Guid>> Login([FromBody] LoginDto loginDto)
        {
            if (loginDto == null)
            {
                return BadRequest("No data to login with");
            }

            Guid id = _authenticatorService.Login(loginDto);

            return Ok(new ApiResponse { RequestName = "Login", TicketId = id });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("add-user")]
        public async Task<ActionResult<Guid>> AddUser()
        {
            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("remove-user")]
        public async Task<ActionResult<Guid>> RemoveUser()
        {
            return Ok();
        }

        private string RetreiveAuthEndpoint() {
            return _configuration.GetSection("InternalAuthenticatorEndpoint")?.Value;
        }
    }
}


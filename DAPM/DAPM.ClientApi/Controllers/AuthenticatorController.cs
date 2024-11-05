using DAPM.ClientApi.Models;
using DAPM.ClientApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.ObjectPool;
using Newtonsoft.Json;
using RabbitMQLibrary.Messages.Authenticator.Base;
using RabbitMQLibrary.Models;
using System.Data;
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

            if (!registerDto.Roles.Contains("Standard"))
            {
                registerDto.Roles.Add("Standard");
            }

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
        [HttpPost("DeleteUser/{username}")]
        public async Task<ActionResult<Guid>> DeleteUser(string username)
        {
            Guid id = _authenticatorService.DeleteUser(username);

            return Ok(new ApiResponse { RequestName = "DeleteUser", TicketId = id });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("GetRoles")]
        public async Task<ActionResult<Guid>> GetRoles()
        {
            Guid id = _authenticatorService.GetRoles();
            
            return Ok(new ApiResponse { RequestName = "GetRoles", TicketId = id });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("AddRoles")]
        public async Task<ActionResult<Guid>> AddRoles(List<string> roles)
        {
            Guid id = _authenticatorService.AddRoles(roles);

            return Ok(new ApiResponse { RequestName = "AddRoles", TicketId = id });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("setOrg/{username}")]
        public async Task<ActionResult<Guid>> SetOrganization([FromBody] OrganisationsDto organisationsDto, string username)
        {
            Guid id = _authenticatorService.SetOrganization(organisationsDto.OrganizationId, organisationsDto.OrganizationName, username);

            return Ok(new ApiResponse { RequestName = "SetOrganization", TicketId = id });
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("EditAsAdmin")]
        public async Task<ActionResult<Guid>> EditAsAdmin(UserEditDto userEditDto)
        {
            if (userEditDto == null)
            {
                return BadRequest("In order to edit a user the edit dto needs to be included");
            }

            Guid id = _authenticatorService.EditAsAdmin(userEditDto);

            return Ok(new ApiResponse { RequestName = "EditAsAdmin", TicketId = id });
        }

        [HttpPut("EditAsUser")]
        public async Task<ActionResult<Guid>> EditAsUser(UserEditDto userEditDto)
        {
            if (userEditDto == null)
            {
                return BadRequest("In order to edit a user the edit dto needs to be included");
            }

            Guid id = _authenticatorService.EditAsUser(userEditDto);

            return Ok(new ApiResponse { RequestName = "EditAsUser", TicketId = id });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("setRoles/{username}")]
        public async Task<ActionResult<Guid>> SetRoles([FromBody] List<string> listofroles, string username)
        {
            if (listofroles == null)
            {
                return BadRequest("payload empty");
            }

            Guid id = _authenticatorService.SetRoles(username, listofroles);

            return Ok(new ApiResponse { RequestName = "SetRoles", TicketId = id });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("GetUsers")]
        public async Task<ActionResult<Guid>> GetUsers()
        {
            Guid id = _authenticatorService.GetUsers();

            return Ok(new ApiResponse { RequestName = "GetUsers", TicketId = id });
        }
    }
}


using DAPM.ClientApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.ObjectPool;
using Newtonsoft.Json;
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

        //private IAuthenticatorService _authenticationService;

        public AuthenticatorController(
            ILogger<AuthenticatorController> logger, 
            IHttpContextAccessor contextAccessor,
            HttpClient httpClient,
            IIdentityService identityService,
            IConfiguration configuration) : base(contextAccessor)
        {
            _logger = logger;
            _httpClient = httpClient;
            _identityService = identityService;
            _configuration = configuration;
            //_authenticationService = authenticationService;
        }

        [HttpPost("sign-up")]
        public async Task<ActionResult<Guid>> SignUp([FromBody] RegistrationDto registerDto)
        {

            Identity identity = _identityService.GetIdentity();

            if (identity == null) {
                return StatusCode(500, "Failed to retrieve peer identity");
            }

            registerDto.OrganizationId = identity.Id.Value;
            registerDto.OrganizationName = identity.Name;

            //send login request to authenticator endpoint
            string authEndpoint = RetreiveAuthEndpoint();

            if (registerDto == null)
            {
                return BadRequest("No data to register with");
            }
            if (authEndpoint == null)
            {
                return StatusCode(500, "Authorizer container endpoint not present");
            }

            string jsonPayload = JsonConvert.SerializeObject(registerDto);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
            var loginEndpoint = $"{authEndpoint}/register";
            var response = await _httpClient.PostAsync(loginEndpoint, content);

            if (!response.IsSuccessStatusCode)
            {
                return BadRequest("registration failed");
            }
            else
            {
                return Ok(await response.Content.ReadAsStringAsync());
            }
        }

        [HttpPost("log-in")]
        public async Task<ActionResult<Guid>> LogIn([FromBody] LoginDto loginDto)
        {
            //send login request to authenticator endpoint
            string authEndpoint = RetreiveAuthEndpoint();

            if (loginDto == null)
            {
                return BadRequest("No data to login with");
            }
            if (authEndpoint == null) {
                return StatusCode(500, "Authorizer container endpoint not present");
            }

            string jsonPayload = JsonConvert.SerializeObject(loginDto);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
            var loginEndpoint = $"{authEndpoint}/login";
            var response = await _httpClient.PostAsync(loginEndpoint, content);

            if (!response.IsSuccessStatusCode)
            {
                return BadRequest("login failed");
            }
            else { 
                return Ok(await response.Content.ReadAsStringAsync()); 
            }
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


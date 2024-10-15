using DAPM.ClientApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using UtilLibrary;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DAPM.ClientApi.Controllers
{
    
    [ApiController]
    [EnableCors("AllowAll")]
    [Route("auth")]
    public class AuthenticatorController : BaseController
    {
        private readonly ILogger<AuthenticatorController> _logger;
        //private IAuthenticatorService _authenticationService;

        public AuthenticatorController(ILogger<AuthenticatorController> logger, IHttpContextAccessor contextAccessor) : base(contextAccessor)
        {
            _logger = logger;
            //_authenticationService = authenticationService;
        }

        [HttpPost("sign-up")]
        public async Task<ActionResult<Guid>> SignUp()
        {
            return Ok();
        }

        [HttpGet("log-in")]
        public async Task<ActionResult<Guid>> LogIn()
        {

            UserDto user;

            return Ok();
        }

        [HttpPost("add-user")]
        public async Task<ActionResult<Guid>> AddUser()
        {
            return Ok();
        }

        
        [HttpDelete("remove-user")]
        public async Task<ActionResult<Guid>> RemoveUser()
        {
            return Ok();
        }


        [Authorize(Roles = "Standard")]
        [HttpGet("test3")]
        public async Task<ActionResult<Guid>> TestRoleGating()
        {
            return Ok("Endpoint accessed");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("test4")]
        public async Task<ActionResult<Guid>> TestRoleGating2()
        {
            return Ok("Endpoint accessed");
        }

        [HttpGet("test5")]
        public async Task<ActionResult<Guid>> showorg()
        {
            return Ok($"you work for {Organization}");
        }
    }
}


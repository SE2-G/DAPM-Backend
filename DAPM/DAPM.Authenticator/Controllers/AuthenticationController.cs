using AutoMapper;
using DAPM.Authenticator.Interfaces.Repostory_Interfaces;
using DAPM.Authenticator.Models;
using DAPM.Authenticator.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UtilLibrary;
using UtilLibrary.Interfaces;
using UtilLibrary.models;

namespace DAPM.Authenticator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly TokenService _tokenService;
        private readonly IIdentityService _identityService;
        private readonly IUserRepository _userrepository;

        public AuthenticationController(
            IMapper mapper,
            IConfiguration configuration,
            UserManager<User> userManager,
            IUserRepository userRepository,
            RoleManager<Role> roleManager,
            TokenService tokenService,
            IIdentityService identityService)
        {
            _mapper = mapper;
            _configuration = configuration;
            _userManager = userManager;
            _roleManager = roleManager;
            _tokenService = tokenService;
            _identityService = identityService;
            _userrepository = userRepository;
        }


        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegistrationDto registerDto)
        {
            User user = _mapper.Map<User>(registerDto);

            if (!UserExists(registerDto.UserName))
            {
                var registerUserResult = await _userManager.CreateAsync(user, registerDto.Password);
                if (!registerUserResult.Succeeded) return BadRequest(registerUserResult.Errors);

                foreach (var role in registerDto.Roles) {
                    var addRoleResult = await _userManager.AddToRoleAsync(user, role);
                    if (!addRoleResult.Succeeded) return BadRequest(addRoleResult.Errors);
                }

                Identity identity = _identityService.GetIdentity();

                user.OrganizationId = (Guid)identity.Id;
                user.OrganizationName = identity.Name;
                _userrepository.SaveChanges(user);
            }
            else
            {
                return BadRequest("Username is already in use");
            }

            return Ok("okily dokily");
        }


        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            User retreival = await _userManager.FindByNameAsync(loginDto.UserName);
            if (retreival == null)
            {
                return Unauthorized("This user does not exist in our system");
            }

            bool result = await _userManager.CheckPasswordAsync(retreival, loginDto.Password);

            if (!result)
            {
                return Unauthorized("Ínvalid password");

            }
            else { 
            
                UserDto response = _mapper.Map<UserDto>(retreival);
                List<string> roles = [.. await _userManager.GetRolesAsync(retreival)];
                string token = await _tokenService.CreateToken(retreival);
                response.Roles = roles;
                response.Token = token;
                return Ok(response);
            }
        }

        [Authorize(Roles = "Standard")]
        [HttpPost("test1")]
        public async Task<IActionResult> TestStandardRole()
        {
            return Ok("Endpoint accessed");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("test2")]
        public async Task<IActionResult> TestAdminRole()
        {
            return Ok("Endpoint accessed");
        }



        private bool UserExists(string name) {
            User result = _userManager.FindByNameAsync(name).GetAwaiter().GetResult();
            if (result == null) { 
                return false;
            }
            return true;
        } 
    }
}

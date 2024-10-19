using AutoMapper;
using DAPM.Authenticator.Interfaces.Repostory_Interfaces;
using DAPM.Authenticator.Models;
using DAPM.Authenticator.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UtilLibrary;

namespace DAPM.Authenticator.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class UserManagementController : ControllerBase
    {
        private readonly RoleManager<Role> _rolemanager;
        private readonly UserManager<User> _usermanager;
        private readonly IUserRepository _userrepository;

        public UserManagementController(RoleManager<Role> roleManager, UserManager<User> usermanager, IUserRepository userRepository)
        {
            _rolemanager = roleManager;
            _usermanager = usermanager;
            _userrepository = userRepository;
        }

        //we dont need to check whether the org exists here, the contacter (DAPM.Client) will not set using a org that doesn't exist
        [HttpPost("setOrg/{username}")]
        public async Task<IActionResult> SetOrganizationOfUser([FromBody] OrganisationsDto organisationsDto, string username) {

            User retreival = await _usermanager.FindByNameAsync(username);
            if (retreival == null)
            {
                return Unauthorized("This user does not exist in our system, therefore we cannot set their org");
            }

            retreival.OrganizationId = organisationsDto.OrganizationId;
            retreival.OrganizationName = organisationsDto.OrganizationName;

            _userrepository.SaveChanges(retreival);

            return Ok($"successfully assigned {username} a new organisation");
        }


        [HttpPost("setRoles/{username}")]
        public async Task<IActionResult> SetRoles([FromBody] List<string> listofroles, string username)
        {
            if (listofroles == null)
            {
                return BadRequest("payload empty");
            }

            User retreival = await _usermanager.FindByNameAsync(username);
            if (retreival == null)
            {
                return Unauthorized("This user does not exist in our system, therefore we cannot add roles to them");
            }

            foreach (string role in listofroles) {
                Role result = await _rolemanager.FindByNameAsync(role);
                if (result == null)
                {
                    return BadRequest("Attempting to add non existent role, all roles in list must exist");
                }              
            }

            foreach (var item in listofroles)
            {
                await _usermanager.AddToRoleAsync(retreival,item); 
            }

            return Ok("succesfully added specified roles");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("Delete")]
        public async Task<IActionResult> DeleteUser(string username)
        {
            User retreival = await _usermanager.FindByNameAsync(username);
            if (retreival == null)
            {
                return BadRequest("This user does not exist in our system, therefore cannot be deleted");
            }

            var result = await _usermanager.DeleteAsync(retreival);

            if (result.Succeeded)
            {

                return Ok("User succesfully deleted");
            }
            else
            {

                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }
        }
    }
}

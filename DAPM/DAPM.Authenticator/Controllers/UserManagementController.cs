using AutoMapper;
using DAPM.Authenticator.Interfaces.Repostory_Interfaces;
using DAPM.Authenticator.Models;
using DAPM.Authenticator.Repositories;
using DAPM.Authenticator.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using UtilLibrary;

namespace DAPM.Authenticator.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class UserManagementController : BaseController
    {
        private readonly RoleManager<Role> _rolemanager;
        private readonly UserManager<User> _usermanager;
        private readonly IUserRepository _userrepository;
        private readonly IMapper _mapper;

        public UserManagementController(
            RoleManager<Role> roleManager, 
            UserManager<User> usermanager, 
            IUserRepository userRepository, 
            IHttpContextAccessor contextAccessor,
            IMapper mapper): base (contextAccessor)
        {
            _rolemanager = roleManager;
            _usermanager = usermanager;
            _userrepository = userRepository;
            _mapper = mapper;
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

        [Authorize(Roles = "Admin")]
        [HttpPut("EditAsAdmin")]
        public async Task<IActionResult> EditUser(UserEditDto userEditDto ) {

            if (userEditDto == null) {
                return BadRequest("In order to edit a user the edit dto needs to be included");
            }

            var user = await _usermanager.FindByIdAsync(userEditDto.Id.ToString());

            if (user == null)
            {
                return BadRequest("Attempt to edit user not in database");
            }


            (bool, string) result = await EditUserWithEditDto(userEditDto, user, _usermanager, _rolemanager, _userrepository);

            if (result.Item1)
            {
                return Ok(result.Item2);
            }
            else { 
                return BadRequest(result.Item2);
            }        
        }


        [Authorize(Roles = "User")]
        [HttpPut("EditAsUser")]
        public async Task<IActionResult> EditUser2(UserEditDto userEditDto)
        {

            if (userEditDto == null)
            {
                return BadRequest("In order to edit a user the edit dto needs to be included");
            }

            var user = await _usermanager.FindByIdAsync(userEditDto.Id.ToString());

            if (user == null)
            {
                return BadRequest("Attempt to edit user not in database");
            }


            //check whether user is allowed to alter this user

            string userIdstring = userId;
            if (userIdstring != null)
            {
                int.TryParse(userIdstring, out int id);

                if (userEditDto.Id == id)
                {
                    (bool, string) result = await EditUserWithEditDto(userEditDto, user, _usermanager, _rolemanager, _userrepository);

                    if (result.Item1)
                    {
                        return Ok(result.Item2);
                    }
                    else
                    {
                        return BadRequest(result.Item2);
                    }
                }
                else {
                    return BadRequest("You are not authorized to edit this user");
                }
            }
            else {
                return BadRequest("You are not authorized to edit this user");
            }
        }

        private async Task<(bool, string)> EditUserWithEditDto(
            UserEditDto editDto, 
            User user, 
            UserManager<User> userManager, 
            RoleManager<Role> rolemanager,
            IUserRepository repository)  {

            try
            {
                user.FullName = editDto.FullName;
                user.UserName = editDto.UserName;

                repository.SaveChanges(user);


                foreach (var role in editDto.Roles)
                {
                    //get all roles not present in new list
                    List<string> currentRoles = userManager.GetRolesAsync(user).GetAwaiter().GetResult().ToList();
                    List<string> rolesThatNeedToBeRemoved = currentRoles.Except(editDto.Roles).ToList();

                    //remove the roles not present in new list
                    foreach (string removerole in rolesThatNeedToBeRemoved) {
                        await userManager.RemoveFromRoleAsync(user, removerole);
                    }
                    
                    //add all new roles, duplicates will sort themselves out
                    if (await rolemanager.RoleExistsAsync(role))
                    {
                        IdentityResult resultrole = await userManager.AddToRoleAsync(user, role);
                        if (resultrole != IdentityResult.Success)
                        {
                            return (false, $"Error occurred when adding role: {role}");
                        }
                    }
                }
                if (editDto.NewPassword != "")
                {
                    IdentityResult resultremove = await userManager.RemovePasswordAsync(user);
                    IdentityResult resultadd = await userManager.AddPasswordAsync(user, editDto.NewPassword);
                    if (resultadd != IdentityResult.Success || resultadd != IdentityResult.Success)
                    {
                        return (false, "Error occurred changing password");
                    }
                }

                return (true, "Edit operation succeeded");
            }
            catch {
                return (false, "Edit operation encountered an exception");
            }


        }

        [Authorize(Roles = "Admin")]
        [HttpGet("GetUsers")]
        public async Task<IActionResult> ReturnAllUsers() {
            List<User> users = _userrepository.Users();

            List<UserDto> usersdto = users.Select (x => {
                UserDto userdtoresp = _mapper.Map<UserDto>(x);
                userdtoresp.Roles = [.. _usermanager.GetRolesAsync(x).GetAwaiter().GetResult()];
                return userdtoresp;
                }).ToList();

            return Ok(usersdto);
        } 

    }
}

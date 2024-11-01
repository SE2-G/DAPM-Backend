using DAPM.Authenticator.Interfaces.Repostory_Interfaces;
using DAPM.Authenticator.Models;
using DAPM.Authenticator.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UtilLibrary;

namespace DAPM.Authenticator.Controllers
{

    [Route("api/[controller]")]
    public class RoleManagementController : BaseController
    {
        private readonly RoleManager<Role> _rolemanager;
        private readonly IRoleRepository _rolerepository;

        public RoleManagementController(RoleManager<Role> roleManager, IRoleRepository roleRepository, IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            _rolemanager = roleManager;
            _rolerepository = roleRepository;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("GetRoles")]
        public async Task<IActionResult> ReturnAllRoles()
        {
            List<Role> roles = _rolerepository.Roles();
            return Ok(roles.Select(role => new { RoleName = role.Name}));
        }


        [Authorize(Roles = "Admin")]
        [HttpPost("AddRoles")]
        public async Task<IActionResult> AddRoles(List<string> roles)
        {
            try
            {
                foreach (var role in roles)
                {
                    await _rolemanager.CreateAsync(new Role { Name = role });
                }
                return Ok("Added the roles");
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }    
        }


    }
}

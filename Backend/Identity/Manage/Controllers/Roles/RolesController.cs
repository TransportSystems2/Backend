using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using System.Threading.Tasks;
using TransportSystems.Backend.Identity.Core.Data.Domain;
using TransportSystems.Backend.Identity.Core.Data.External.Users;
using TransportSystems.Backend.Identity.Core.Interfaces;

namespace TransportSystems.Backend.Identity.Manage.Controllers.Roles
{
    [Route("identity/manage/[controller]"), Authorize]
    public class RolesController : Controller
    {
        public RolesController(IRoleService roleService)
        {
            RoleService = roleService;
        }

        private IRoleService RoleService { get; }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Json(RoleService.Roles);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody]UserRoleModel model)
        {
            var role = new UserRole(model.Name);

            var result = await RoleService.CreateAsync(role);
            if (result.Succeeded)
            {
                return Ok();
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }

        [HttpPost("delete")]
        public async Task<IActionResult> Delete([FromBody]UserRoleModel model)
        {
            var role = await RoleService.FindByNameAsync(model.Name);
            if (role == null)
            {
                return NotFound();
            }

            var result = await RoleService.DeleteAsync(role);
            if (result.Succeeded)
            {
                return Ok();
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }
    }
}

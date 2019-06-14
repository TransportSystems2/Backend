using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;
using IdentityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using TransportSystems.Backend.Identity.Core.Data.Domain;
using TransportSystems.Backend.Identity.Core.Data.External.Users;
using TransportSystems.Backend.Identity.Core.Interfaces;

namespace TransportSystems.Backend.Identity.Manage.Controllers.Users
{
    [Route("identity/manage/[controller]")]
    public class UsersController : Controller
    {
        public UsersController(IMapper mapper, IUserService userService, IRoleService roleService, ILogger<UsersController> logger)
        {
            Mapper = mapper;
            UserService = userService;
            RoleService = roleService;
            Logger = logger;
        }

        public IMapper Mapper { get; }

        public IUserService UserService { get; }

        public IRoleService RoleService { get; }

        private ILogger<UsersController> Logger { get; }

        [HttpPost("create/")]
        public async virtual Task<IActionResult> Create([FromBody] UserModel userModel)
        {
            var user = Mapper.Map<User>(userModel);
            user.SecurityStamp = userModel.PhoneNumber.ToSha256();

            var result = await UserService.CreateAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok(user);
        }

        [HttpPost("existbyid/{id}")]
        public async virtual Task<IActionResult> ExistById(int id)
        {
            var result = await UserService.ExistAsync(id);

            return Json(result);
        }

        [HttpPost("existbyphonenumber/{phoneNumber}")]
        public virtual IActionResult ExistByPhoneNumber(string phoneNumber)
        {
            var user = UserService.FindByPhoneNumber(phoneNumber);
            var result = user != null;

            return Json(result);
        }

        [HttpPost("isinrole/{id}")]
        public async virtual Task<IActionResult> InRole(int id, [FromBody] string role)
        {
            var result = await UserService.IsInRoleAsync(id, role);

            return Json(result);
        }

        [HttpPost("isundefined/{id}")]
        public async virtual Task<IActionResult> IsUndefined(int id)
        {
            if (await UserService.IsInRoleAsync(id, UserRole.AdminRoleName))
            {
                return Json(false);
            }

            if (await UserService.IsInRoleAsync(id, UserRole.ModeratorRoleName))
            {
                return Json(false);
            }

            if (await UserService.IsInRoleAsync(id, UserRole.DispatcherRoleName))
            {
                return Json(false);
            }

            if (await UserService.IsInRoleAsync(id, UserRole.DriverRoleName))
            {
                return Json(false);
            }

            if (await UserService.IsInRoleAsync(id, UserRole.CustomerRoleName))
            {
                return Json(false);
            }

            return Json(true);
        }

        [HttpPut("update/{id}")]
        public async virtual Task<IActionResult> Update(int id, [FromBody]UserModel userModel)
        {
            var user = await UserService.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound($"not found user with id: {id}");
            }

            if (!string.IsNullOrEmpty(userModel.PhoneNumber))
            {
                user.UserName = userModel.PhoneNumber;
            }

            if (!string.IsNullOrEmpty(userModel.Email))
            {
                user.Email = userModel.Email;
            }

            if (!string.IsNullOrEmpty(userModel.PhoneNumber))
            {
                user.PhoneNumber = userModel.PhoneNumber;
            }

            if (!string.IsNullOrEmpty(userModel.FirstName))
            {
                user.FirstName = userModel.FirstName;
            }

            if (!string.IsNullOrEmpty(userModel.LastName))
            {
                user.LastName = userModel.LastName;
            }

            if (userModel.CompanyId != 0)
            {
                user.CompanyId = userModel.CompanyId;
            }

            if (userModel.VehicleId != 0)
            {
                user.VehicleId = userModel.VehicleId;
            }

            var result = await UserService.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok("user is updated");
        }

        [HttpPost("all/")]
        public virtual IActionResult GetAll()
        {
            if (UserService.Users == null || !UserService.Users.Any())
            {
                return NotFound();
            }

            var users = UserService.Users.AsEnumerable();
            IEnumerable<UserModel> collection = Mapper.Map<IEnumerable<User>, IEnumerable<UserModel>>(users);

            return Json(collection);
        }

        [HttpPost("{id}")]
        public async virtual Task<IActionResult> Get(int id)
        {
            var user = await UserService.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound($"not found user with id: {id}");
            }

            var userModel = Mapper.Map<UserModel>(user);

            return Json(userModel);
        }

        [HttpPost]
        public virtual IActionResult Get([FromBody] int[] ids)
        {
            var users = UserService.FindByIds(ids);
            if (users == null || !users.Any())
            {
                return NotFound("not found users with specific ids");
            }

            IEnumerable<UserModel> collection = Mapper.Map<IEnumerable<User>, IEnumerable<UserModel>>(users);

            return Json(collection);
        }

        [HttpPost("asigntoroles/{userId}")]
        public async virtual Task<IActionResult> AsignToRoles(int userId, [FromBody] string[] roles)
        {
            var user = await UserService.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"not found user with id: {userId}");
            }

            var result = await AsignRolesToUser(user, roles);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok($"user id: {userId} roles was updated");
        }

        [HttpPost("byrole/{role}")]
        public async virtual Task<IActionResult> GetWithRole(string role)
        {
            var userModels = await GetUserModelsByRole(role);
            if (userModels == null || !userModels.Any())
            {
                return NotFound($"not found users with role: {role}");
            }

            return Json(userModels);
        }

        [HttpPost("byroles/")]
        public async virtual Task<IActionResult> GetWithRoles([FromBody] string[] roles)
        {
            if (!roles.Any())
            {
                return NotFound($"roles is empty");
            }

            var userModels = new List<UserModel>();
            foreach (var role in roles)
            {
                var tmpUsersModel = await GetUserModelsByRole(role);
                if (tmpUsersModel != null && tmpUsersModel.Any())
                {
                    userModels.AddRange(tmpUsersModel);
                }
            }

            if (!userModels.Any())
            {
                return NotFound("not found users with specific roles");
            }

            return Json(userModels);
        }

        [HttpPost("byphonenumber/{phoneNumber}")]
        public virtual IActionResult GetByPhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrEmpty(phoneNumber))
            {
                return NotFound($"phoneNumber is empty");
            }

            var user = UserService.FindByPhoneNumber(phoneNumber);
            if (user == null)
            {
                return NotFound($"not found user with phonenumber: {phoneNumber}");
            }

            var userModel = Mapper.Map<UserModel>(user);

            return Json(userModel);
        }

        private async Task<IEnumerable<UserModel>> GetUserModelsByRole(string role)
        {
            var userModels = new List<UserModel>();
            if (string.IsNullOrEmpty(role))
            {
                Logger.LogWarning("role is empty");

                return userModels;
            }

            var result = await RoleService.FindByNameAsync(role);
            if (result == null)
            {
                Logger.LogWarning($"not found role: {role}");

                return userModels;
            }

            var users = await UserService.GetUsersInRoleAsync(role);
            if (users == null || !users.Any())
            {
                Logger.LogWarning($"not found users with role: {role}");

                return userModels;
            }

            var mappedModels = Mapper.Map<IEnumerable<User>, IEnumerable<UserModel>>(users);
            userModels.AddRange(mappedModels);

            return userModels;
        }

        /// TODO: разделить на два метода, AddToRolesAsync и RemoveFromRolesAsync.
        /// Удобней вызывать API с добавлением или удалением конкретных ролей,
        /// а не запрашивать все роли для клиента и удалять/добавлять нужные.
        private async Task<IdentityResult> AsignRolesToUser(User user, string[] roles)
        {
            if (roles == null)
            {
                return IdentityResult.Success;
            }

            var userRoles = await UserService.GetRolesAsync(user);
            var allRoles = RoleService.Roles.ToList();
            var addedRoles = roles.Except(userRoles);
            var removedRoles = userRoles.Except(roles);

            var result = await UserService.AddToRolesAsync(user, addedRoles);
            if (result.Succeeded)
            {
                result = await UserService.RemoveFromRolesAsync(user, removedRoles);
            }

            return result;
        }
    }
}
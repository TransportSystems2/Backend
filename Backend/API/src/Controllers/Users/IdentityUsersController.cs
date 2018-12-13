using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using TransportSystems.Backend.Core.Domain.Core.Users;
using TransportSystems.Backend.Core.Services.Interfaces.Users;

namespace TransportSystems.Backend.API.Controllers.Users
{
    [Route("api/[controller]")]
    public class IdentityUsersController : Controller
    {
        public IdentityUsersController(IIdentityUserService identityUserService)
        {
            IdentityUserService = identityUserService;
        }

        protected IIdentityUserService IdentityUserService { get; }

        /// <summary>
        /// Список Identity пользователей
        /// </summary>
        [HttpGet]
        public async Task<ICollection<IdentityUser>> Get()
        {
            return await IdentityUserService.GetUsers();
        }

        /// <summary>
        /// Поиск Identity пользователя по Id
        /// </summary>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var user = IdentityUserService.GetUser(id);

            if (user != null)
            {
                return Ok(user);
            }
            else
            {
                return NotFound("Not found");
            }
        }

        /// <summary>
        /// Создаление Identity пользователя
        /// </summary>
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        /// <summary>
        /// Обновление Identity пользователя
        /// </summary>
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        /// <summary>
        /// Удаление Identity пользователя
        /// </summary>
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}

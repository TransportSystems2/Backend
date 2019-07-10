using System.Collections.Generic;
using System.Threading.Tasks;

using Refit;

using TransportSystems.Backend.Identity.Core.Data.External.Users;

namespace TransportSystems.Backend.Identity.API
{
    public interface IIdentityUsersAPI
    {
        [Post("/identity/manage/users/create")]
        Task<UserModel> Create([Body(BodySerializationMethod.Json)] UserModel userModel);

        [Post("/identity/manage/users/all")]
        Task<ICollection<UserModel>> GetAllAsync();

        [Get("/identity/manage/users/bycompany/{companyId}/{role}")]
        Task<ICollection<UserModel>> GetByCompany(int companyId, string role);

        [Post("/identity/manage/users/{id}")]
        Task<UserModel> ByIdAsync(int id);

        [Post("/identity/manage/users")]
        Task<ICollection<UserModel>> ByIdsAsync(int[] ids);

        [Post("/identity/manage/users/asigntoroles/{userId}")]
        Task AsignToRoles(int userId, [Body(BodySerializationMethod.Json)] string[] roles);

        [Post("/identity/manage/users/byrole/{role}")]
        Task<ICollection<UserModel>> ByRole(string role);

        [Post("/identity/manage/users/byroles")]
        Task<ICollection<UserModel>> ByRoles(string[] roles);

        [Post("/identity/manage/users/byphonenumber/{phoneNumber}")]
        Task<UserModel> FindByPhoneNumberAsync(string phoneNumber);

        [Post("/identity/manage/users/existbyid/{id}")]
        Task<bool> IsExistById(int id);

        [Post("/identity/manage/users/existbyphonenumber/{phoneNumber}")]
        Task<bool> IsExistByPhoneNumber(string phoneNumber);

        [Post("/identity/manage/users/isinrole/{id}")]
        Task<bool> IsInRole(int id, [Body(BodySerializationMethod.Json)] string role);

        [Post("/identity/manage/users/isundefined/{id}")]
        Task<bool> IsUndefined(int id);

        [Put("/identity/manage/users/update/{id}")]
        Task Update(int id, [Body(BodySerializationMethod.Json)] UserModel user);
    }
}
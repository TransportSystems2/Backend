using AutoMapper;

using TransportSystems.Backend.Core.Domain.Core.Users;
using TransportSystems.Backend.Identity.Core.Data.External.Users;

namespace TransportSystems.Backend.API.Mapping
{
    public class UsersProfile : Profile
    {
        public UsersProfile()
        {
            CreateMap<UserModel, IdentityUser>()
                .ForMember(x => x.AddedDate, opt => opt.Ignore())
                .ForMember(x => x.ModifiedDate, opt => opt.Ignore());

            CreateMap<Moderator, UserModel>();
            CreateMap<Dispatcher, UserModel>();
            CreateMap<Driver, UserModel>();
            CreateMap<Customer, UserModel>();
        }
    }
}
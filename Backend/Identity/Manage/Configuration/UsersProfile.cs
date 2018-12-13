using AutoMapper;

using TransportSystems.Backend.Identity.Core.Data.Domain;
using TransportSystems.Backend.Identity.Core.Data.External.Users;

namespace TransportSystems.Backend.Identity.Manage.Configuration
{
    public class UsersProfile : Profile
    {
        public UsersProfile()
        {
            CreateMap<UserModel, User>()
                .ForMember(x => x.UserName, opt => opt.MapFrom(s => s.PhoneNumber))
                .ForMember(x => x.NormalizedUserName, opt => opt.Ignore())
                .ForMember(x => x.NormalizedEmail, opt => opt.Ignore())
                .ForMember(x => x.EmailConfirmed, opt => opt.Ignore())
                .ForMember(x => x.PasswordHash, opt => opt.Ignore())
                .ForMember(x => x.SecurityStamp, opt => opt.Ignore())
                .ForMember(x => x.ConcurrencyStamp, opt => opt.Ignore())
                .ForMember(x => x.PhoneNumberConfirmed, opt => opt.Ignore())
                .ForMember(x => x.TwoFactorEnabled, opt => opt.Ignore())
                .ForMember(x => x.LockoutEnd, opt => opt.Ignore())
                .ForMember(x => x.LockoutEnabled, opt => opt.Ignore())
                .ForMember(x => x.AccessFailedCount, opt => opt.Ignore());
        }
    }
}
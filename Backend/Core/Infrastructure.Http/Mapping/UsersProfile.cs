using AutoMapper;

using TransportSystems.Backend.Core.Domain.Core.Users;
using TransportSystems.Backend.Identity.Core.Data.External.Users;

namespace TransportSystems.Backend.Core.Infrastructure.Http.Mapping
{
    public class UsersProfile : Profile
    {
        public UsersProfile()
        {
            CreateMap<UserModel, Moderator>()
                .ForMember(x => x.AddedDate, opt => opt.Ignore())
                .ForMember(x => x.ModifiedDate, opt => opt.Ignore());

            CreateMap<UserModel, Dispatcher>()
                .ForMember(x => x.AddedDate, opt => opt.Ignore())
                .ForMember(x => x.ModifiedDate, opt => opt.Ignore());

            CreateMap<UserModel, Driver>()
                .ForMember(x => x.AddedDate, opt => opt.Ignore())
                .ForMember(x => x.ModifiedDate, opt => opt.Ignore());

            CreateMap<UserModel, Customer>()
                .ForMember(x => x.AddedDate, opt => opt.Ignore())
                .ForMember(x => x.ModifiedDate, opt => opt.Ignore());
            CreateMap<Customer, UserModel>()
                .ForMember(x => x.CompanyId, opt => opt.Ignore());
        }
    }
}
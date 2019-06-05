using AutoMapper;
using TransportSystems.Backend.Core.Domain.Core.Geo;
using TransportSystems.Backend.Core.Domain.Core.Transport;
using TransportSystems.Backend.External.Models.Geo;
using TransportSystems.Backend.Application.Models.Geo;
using TransportSystems.Backend.Application.Models.Transport;
using TransportSystems.Backend.Core.Domain.Core.Catalogs;
using TransportSystems.Backend.Application.Models.Catalogs;
using TransportSystems.Backend.Application.Models.Billing;
using TransportSystems.Backend.Core.Domain.Core.Billing;
using TransportSystems.Backend.Core.Domain.Core.Users;
using TransportSystems.Backend.Application.Models.Users;

namespace TransportSystems.Backend.Application.Business.Mapping
{
    public class ApplicationMappingProfile : Profile
    {
        public ApplicationMappingProfile()
        {
            CreateMap<Cargo, CargoAM>();

            CreateMap<Address, AddressAM>();
            CreateMap<AddressEM, AddressAM>();

            CreateMap<CatalogItem, CatalogItemAM>();

            CreateMap<Basket, BasketAM>();
            CreateMap<BillItem, BillItemAM>();

            CreateMap<Customer, CustomerAM>();
        }
    }
}
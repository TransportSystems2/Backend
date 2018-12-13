using System.Collections.Generic;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Geo;
using TransportSystems.Backend.Core.Domain.Interfaces;

namespace TransportSystems.Backend.Core.Services.Interfaces.Geo
{
    public interface IAddressService : IDomainService<Address>
    {
        Task<Address> Create(
            AddressKind kind,
            string request,
            string country,
            string province,
            string area,
            string locality,
            string district,
            string street,
            string house,
            double latitude,
            double longitude,
            double adjustedLatitude = 0,
            double adjustedLongitude = 0);

        Task<Address> GetByCoordinate(double latitude, double longitude);

        Task<ICollection<Address>> GetByCoordinateBounds(AddressKind kind, double minLatitude, double minLongitude, double maxLatitude, double maxLongitude);

        Task<string> GetShortTitle(int addressId);

        Task<ICollection<string>> GetProvinces(AddressKind kind,string country,  OrderingKind orderingKind = OrderingKind.None);

        Task<ICollection<string>> GetLocalities(AddressKind kind,string country, string province,  OrderingKind orderingKind = OrderingKind.None);

        Task<ICollection<string>> GetDistricts(AddressKind kind, string country, string province, string locality,  OrderingKind orderingKind = OrderingKind.None);

        Task<ICollection<Address>> GetByGeocoding(AddressKind kind, string country, string province = null, string locality = null, string district = null, string street = null, string house = null);
    }
}
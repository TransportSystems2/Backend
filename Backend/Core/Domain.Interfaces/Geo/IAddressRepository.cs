using System.Collections.Generic;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Geo;

namespace TransportSystems.Backend.Core.Domain.Interfaces.Geo
{
    public interface IAddressRepository : IRepository<Address>
    {
        Task<Address> GetByCoordinate(AddressKind kind, double latitude, double longitude);

        Task<ICollection<string>> GetProvinces(AddressKind kind, string country, OrderingKind orderingKind = OrderingKind.None);

        Task<ICollection<string>> GetLocalities(AddressKind kind, string country, string province, OrderingKind orderingKind = OrderingKind.None);

        Task<ICollection<string>> GetDistricts(AddressKind kind, string country, string province, string locality, OrderingKind orderingKind = OrderingKind.None);

        Task<ICollection<Address>> GetByGeocoding(AddressKind kind, string country, string province = null, string locality = null, string district = null, string street = null, string house = null);

        Task<ICollection<Address>> GetInCoordinateBounds(AddressKind kind, double minLatitude, double minLongitude, double maxLatitude, double maxLongitude);
    }
}
using Common.Models;
using Common.Models.Units;
using System.Collections.Generic;
using System.Threading.Tasks;
using TransportSystems.Backend.Application.Models.Geo;
using TransportSystems.Backend.Core.Domain.Core.Geo;
using TransportSystems.Backend.External.Models.Geo;

namespace TransportSystems.Backend.Application.Interfaces.Geo
{
    public interface IApplicationAddressService : IApplicationTransactionService
    {
        Task<Address> GetDomainAddress(int addressId);

        Task<Address> GetDomainAddressByCoordinate(Coordinate coordinate);

        Task<Address> CreateDomainAddress(AddressKind kind, AddressAM address);

        Task<Address> GetOrCreateDomainAddress(AddressAM address);

        Task<AddressAM> GetAddress(int addressId);

        Task<string> GetShortTitle(int addressId);

        Task<ICollection<AddressAM>> GetNearestAddresses(AddressKind kind, Coordinate originCoordinate, double distance = 500, int maxResultCount = 5);

        Task<ICollection<AddressAM>> Geocode(string request, int maxResultCount = 5);

        Task<ICollection<AddressAM>> ReverseGeocode(double latitude, double longitude);

        Task<AddressAM> GetNearestAddress(Coordinate originCoordinate, IEnumerable<AddressAM> addresses);

        AddressAM FromDomainAddress(Address source);

        ICollection<AddressAM> FromExternalAddressess(ICollection<AddressEM> source);

        AddressAM FromExternalAddress(AddressEM source);

        ICollection<AddressAM> FromDomainAddresses(ICollection<Address> source);

        Task<TimeBelt> GetTimeBeltByAddress(AddressAM address);
    }
}
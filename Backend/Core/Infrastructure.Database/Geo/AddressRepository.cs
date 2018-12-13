using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Geo;
using TransportSystems.Backend.Core.Domain.Interfaces;
using TransportSystems.Backend.Core.Domain.Interfaces.Geo;
using TransportSystems.Backend.Core.Infrastructure.Database.Extension;

namespace TransportSystems.Backend.Core.Infrastructure.Database.Geo
{
    public class AddressRepository : Repository<Address>, IAddressRepository
    {
        public AddressRepository(ApplicationContext context)
            : base(context)
        {
        }

        public async Task<ICollection<string>> GetProvinces(AddressKind kind, string country, OrderingKind orderingKind = OrderingKind.None)
        {
            return await Entities.Where(i => i.Kind.Equals(kind) && i.Country.Contains(country)).Select(i => i.Province).Distinct().OrderBy(i => i, orderingKind).ToListAsync();
        }

        public async Task<ICollection<string>> GetLocalities(AddressKind kind, string country, string province, OrderingKind orderingKind = OrderingKind.None)
        {
            return await Entities.Where(i => i.Kind.Equals(kind) && i.Country.Contains(country) && i.Province.Contains(province)).Select(i => i.Locality).Distinct().OrderBy(i => i, orderingKind).ToListAsync();
        }

        public async Task<ICollection<string>> GetDistricts(AddressKind kind, string country, string province, string locality, OrderingKind orderingKind = OrderingKind.None)
        {
            return await Entities.Where(i => i.Kind.Equals(kind) && i.Country.Contains(country) && i.Province.Contains(province) && i.Locality.Contains(locality)).Select(i => i.District).Distinct().OrderBy(i => i, orderingKind).ToListAsync();
        }

        public async Task<ICollection<Address>> GetByGeocoding(AddressKind kind, string country, string province = null, string locality = null, string district = null, string street = null, string house = null)
        {
            return await Entities.Where(i => 
                i.Kind.Equals(kind)
                && country.Equals(i.Country)
                && ((province == null) || province.Equals(i.Province))
                && ((locality == null) || locality.Equals(i.Locality))
                && ((district == null) || district.Equals(i.District))
                && ((street == null) || street.Equals(i.Street))
                ).ToListAsync();
        }

        public async Task<ICollection<Address>> GetInCoordinateBounds(AddressKind kind, double minLatitude, double minLongitude, double maxLatitude, double maxLongitude)
        {
            return await Entities.Where(i =>
                i.Kind.Equals(kind)
                && (i.Latitude >= minLatitude) && (i.Latitude <= maxLatitude)
                && (i.Longitude >= minLongitude) && (i.Longitude <= maxLongitude)).ToListAsync();
        }

        public Task<Address> GetByCoordinate(double latitude, double longitude)
        {
            return Entities.SingleOrDefaultAsync(i => i.Latitude.Equals(latitude) && i.Longitude.Equals(longitude));
        }
    }
}
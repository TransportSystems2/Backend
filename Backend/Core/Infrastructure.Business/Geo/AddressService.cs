using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Geo;
using TransportSystems.Backend.Core.Domain.Interfaces;
using TransportSystems.Backend.Core.Domain.Interfaces.Geo;
using TransportSystems.Backend.Core.Services.Interfaces;
using TransportSystems.Backend.Core.Services.Interfaces.Geo;

namespace TransportSystems.Backend.Core.Infrastructure.Business.Geo
{
    public class AddressService : DomainService<Address>, IAddressService
    {
        public AddressService(IAddressRepository repository)
            : base(repository)
        {
        }

        protected new IAddressRepository Repository => base.Repository as IAddressRepository;

        public Task<Address> Create(
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
            double adjustedLongitude = 0)
        {
            var address = new Address
            {
                Kind = kind,
                Request = request,
                Country = country,
                Province = province,
                Area = area,
                Locality = locality,
                District = district,
                Street = street,
                House = house,
                Latitude = latitude,
                Longitude = longitude,
                AdjustedLatitude = adjustedLatitude,
                AdjustedLongitude = adjustedLongitude
            };

            return Create(address);
        }

        public Task<Address> GetByCoordinate(AddressKind kind, double latitude, double longitude)
        {
            return Repository.GetByCoordinate(kind, latitude, longitude);
        }

        public Task<ICollection<string>> GetProvinces(AddressKind kind, string country, OrderingKind orderingKind = OrderingKind.None)
        {
            if (string.IsNullOrEmpty(country))
            {
                throw new ArgumentNullException("Country");
            }

            return Repository.GetProvinces(kind, country, orderingKind);
        }

        public Task<ICollection<string>> GetLocalities(AddressKind kind, string country, string province, OrderingKind orderingKind = OrderingKind.None)
        {
            if (string.IsNullOrEmpty(country))
            {
                throw new ArgumentNullException("Country");
            }

            if (string.IsNullOrEmpty(province))
            {
                throw new ArgumentNullException("Province");
            }

            return Repository.GetLocalities(kind, country, province, orderingKind);
        }

        public Task<ICollection<string>> GetDistricts(AddressKind kind, string country, string province, string locality, OrderingKind orderingKind = OrderingKind.None)
        {
            if (string.IsNullOrEmpty(country))
            {
                throw new ArgumentNullException("Country");
            }

            if (string.IsNullOrEmpty(province))
            {
                throw new ArgumentNullException("Province");
            }

            if (string.IsNullOrEmpty(locality))
            {
                throw new ArgumentNullException("Locality");
            }

            return Repository.GetDistricts(kind, country, province, locality, orderingKind);
        }

        public async Task<string> GetShortTitle(int addressId)
        {
            var address = await Repository.Get(addressId);
            if (address == null)
            {
                throw new EntityNotFoundException($"AddressId:{addressId} not found", "Id");
            }

            if (string.IsNullOrEmpty(address.Locality))
            {
                throw new ArgumentException($"Locality is null or empty", "Locality");
            }

            return address.Locality;
        }

        public Task<ICollection<Address>> GetByGeocoding(AddressKind kind, string country, string province = null, string locality = null, string district = null, string street = null, string house = null)
        {
            return Repository.GetByGeocoding(kind, country, province, locality, district, street, house);
        }

        public Task<ICollection<Address>> GetByCoordinateBounds(AddressKind kind, double minLatitude, double minLongitude, double maxLatitude, double maxLongitude)
        {
            return Repository.GetInCoordinateBounds(kind, minLatitude, minLongitude, maxLatitude, maxLongitude);
        }

        protected async Task<Address> Create(Address address)
        {
            await Verify(address);

            await Repository.Add(address);
            await Repository.Save();

            return address;
        }

        protected override Task<bool> DoVerifyEntity(Address entity)
        {
            if (string.IsNullOrEmpty(entity.Country))
            {
                throw new ArgumentException($"Country is null or empty", "Country");
            }

            if (string.IsNullOrEmpty(entity.Province))
            {
                throw new ArgumentException($"Province is null or empty", "Province");
            }

            if (string.IsNullOrEmpty(entity.Locality))
            {
                throw new ArgumentException($"Locality is null or empty", "Locality");
            }

            if (entity.Latitude == 0)
            {
                throw new ArgumentException("Latitude");
            }

            if (entity.Longitude == 0)
            {
                throw new ArgumentException("Longitude");
            }

            return Task.FromResult(true);
        }
    }
}
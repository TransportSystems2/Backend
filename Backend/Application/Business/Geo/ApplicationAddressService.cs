using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Models.Geolocation;
using TransportSystems.Backend.Application.Interfaces.Geo;
using TransportSystems.Backend.Application.Interfaces.Mapping;
using TransportSystems.Backend.Application.Models.Geo;
using TransportSystems.Backend.Core.Domain.Core.Geo;
using TransportSystems.Backend.Core.Services.Interfaces;
using TransportSystems.Backend.Core.Services.Interfaces.Geo;
using TransportSystems.Backend.External.Interfaces.Services;
using TransportSystems.Backend.External.Models.Geo;

namespace TransportSystems.Backend.Application.Business.Geo
{
    public class ApplicationAddressService : ApplicationTransactionService, IApplicationAddressService
    {
        public ApplicationAddressService(
            ITransactionService transactionService,
            IMappingService mappingService,
            IAddressService domainAddressService,
            IDirectionService directionService,
            IGeocoderService geocoderService)
            : base(transactionService)
        {
            MappingService = mappingService;
            DomainAddressService = domainAddressService;
            DirectionService = directionService;
            GeocoderService = geocoderService;
        }

        protected IMappingService MappingService { get; }

        protected IAddressService DomainAddressService { get; }

        protected IDirectionService DirectionService { get; }

        protected IGeocoderService GeocoderService { get; }

        public Task<Address> CreateDomainAddress(AddressKind kind, AddressAM address)
        {
            return DomainAddressService.Create(
                    kind,
                    address.Request,
                    address.Country,
                    address.Province,
                    address.Area,
                    address.Locality,
                    address.District,
                    address.Street,
                    address.House,
                    address.Latitude,
                    address.Longitude,
                    address.AdjustedLatitude,
                    address.AdjustedLongitude);
        }

        public async Task<Address> GetOrCreateDomainAddress(AddressAM address)
        {
            var result = await DomainAddressService.GetByCoordinate(address.Latitude, address.Longitude);
            if (result == null)
            {
                result = await CreateDomainAddress(AddressKind.Other, address);
            }

            return result;
        }

        public Task<Address> GetDomainAddress(int addressId)
        {
            return DomainAddressService.Get(addressId); 
        }

        public async Task<AddressAM> GetAddress(int addressId)
        {
            AddressAM result = null;

            var domainAddress = await GetDomainAddress(addressId);

            if (domainAddress != null)
            {
                result = FromDomainAddress(domainAddress);
            }

            return result;
        }

        public Task<string> GetShortTitle(int addressId)
        {
            return DomainAddressService.GetShortTitle(addressId);
        }

        public async Task<AddressAM> GetNearestAddress(Coordinate originCoordinate, IEnumerable<AddressAM> addresses)
        {
            var nearestCoordinate = await DirectionService.GetNearestCoordinate(originCoordinate, addresses.Select(a => a.ToCoordinate()));

            return addresses.First(a => a.Latitude.Equals(nearestCoordinate.Latitude) && a.Longitude.Equals(nearestCoordinate.Longitude));
        }

        public async Task<ICollection<AddressAM>> Geocode(string request, int maxResultCount = 5)
        {
            var externalAddresses = await GeocoderService.Geocode(request, maxResultCount);

            return FromExternalAddressess(externalAddresses);
        }

        public async Task<ICollection<AddressAM>> ReverseGeocode(double latitude, double longitude)
        {
            var externalAddresses = await GeocoderService.ReverseGeocode(latitude, longitude);

            return FromExternalAddressess(externalAddresses);
        }

        public AddressAM FromDomainAddress(Address source)
        {
            var destination = new AddressAM();

            return MappingService.Map(source, destination);
        }

        public ICollection<AddressAM> FromExternalAddressess(ICollection<AddressEM> addresses)
        {
            var result = new List<AddressAM>();
            if (addresses != null)
            {
                result.AddRange(addresses.Select(FromExternalAddress));
            }

            return result;
        }


        public AddressAM FromExternalAddress(AddressEM source)
        {
            var destination = new AddressAM();

            return MappingService.Map(source, destination);
        }

        public async Task<ICollection<AddressAM>> GetNearestAddresses(AddressKind kind, Coordinate originCoordinate, double distance = 500, int maxResultCount = 5)
        {

            var coordinateBounds = await DirectionService.GetCoordinateBounds(originCoordinate, distance);
            var domainAddresses = await DomainAddressService.GetByCoordinateBounds(
                kind,
                coordinateBounds.MinLatitude,
                coordinateBounds.MinLongitude,
                coordinateBounds.MaxLatitude,
                coordinateBounds.MaxLongitude);

            var nearestDomainAddresses = new List<Address>();

            if (domainAddresses.Any())
            {
                var nearestCoordinates = await DirectionService.GetNearestCoordinates(originCoordinate, domainAddresses.Select(a => a.ToCoordinate()), maxResultCount);

                foreach (var coordinate in nearestCoordinates)
                {
                    var domainAddress = domainAddresses.First(a => a.Latitude.Equals(coordinate.Latitude) && a.Longitude.Equals(coordinate.Longitude));
                    nearestDomainAddresses.Add(domainAddress);
                }
            }

            return FromDomainAddresses(nearestDomainAddresses);
        }

        public ICollection<AddressAM> FromDomainAddresses(ICollection<Address> source)
        {
            return source.Select(i => FromDomainAddress(i)).ToList();
        }

        public Task<Address> GetDomainAddressByCoordinate(Coordinate coordinate)
        {
            return DomainAddressService.GetByCoordinate(coordinate.Latitude, coordinate.Longitude);
        }

        public Task<TimeZoneInfo> GetTimeZoneByCoordinate(AddressAM address)
        {
            var moscowTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time");
            return Task.FromResult(moscowTimeZone);
        }
    }
}
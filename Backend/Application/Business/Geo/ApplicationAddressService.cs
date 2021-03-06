﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Models;
using Common.Models.Units;
using TransportSystems.Backend.Application.Interfaces.Geo;
using TransportSystems.Backend.Application.Interfaces.Mapping;
using TransportSystems.Backend.Application.Models.Geo;
using TransportSystems.Backend.Core.Domain.Core.Geo;
using TransportSystems.Backend.Core.Services.Interfaces;
using TransportSystems.Backend.Core.Services.Interfaces.Geo;
using TransportSystems.Backend.External.Interfaces.Services.Direction;
using TransportSystems.Backend.External.Interfaces.Services.Geocoder;
using TransportSystems.Backend.External.Interfaces.Services.Maps;
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
            IGeocoderService geocoderService,
            IMapsService mapsService)
            : base(transactionService)
        {
            MappingService = mappingService;
            DomainAddressService = domainAddressService;
            DirectionService = directionService;
            GeocoderService = geocoderService;
            MapsService = mapsService;
        }

        protected IMappingService MappingService { get; }

        protected IAddressService DomainAddressService { get; }

        protected IDirectionService DirectionService { get; }

        protected IGeocoderService GeocoderService { get; }

        protected IMapsService MapsService { get; }

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

        public async Task<Address> GetOrCreateDomainAddress(AddressKind kind, AddressAM address)
        {
            var result = await DomainAddressService.GetByCoordinate(kind, address.Latitude, address.Longitude);
            if (result == null)
            {
                result = await CreateDomainAddress(kind, address);
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

        public async Task<ICollection<Address>> GetNearestDomainAddresses(AddressKind kind,
            Coordinate originCoordinate,
            double distance = 500,
            int maxResultCount = 5)
        {
            var coordinateBounds = await DirectionService.GetCoordinateBounds(originCoordinate, distance);
            var domainAddresses = await DomainAddressService.GetByCoordinateBounds(
                kind,
                coordinateBounds.MinLatitude,
                coordinateBounds.MinLongitude,
                coordinateBounds.MaxLatitude,
                coordinateBounds.MaxLongitude);

            var result = new List<Address>();

            if (domainAddresses.Any())
            {
                var nearestCoordinates = await DirectionService.GetNearestCoordinates(originCoordinate, domainAddresses.Select(a => a.ToCoordinate()), maxResultCount);

                foreach (var coordinate in nearestCoordinates)
                {
                    var domainAddress = domainAddresses.First(a => a.Latitude.Equals(coordinate.Latitude) && a.Longitude.Equals(coordinate.Longitude));
                    result.Add(domainAddress);
                }
            }

            return result;
        }

        public ICollection<AddressAM> FromDomainAddresses(ICollection<Address> source)
        {
            return source.Select(i => FromDomainAddress(i)).ToList();
        }

        public Task<Address> GetDomainAddressByCoordinate(AddressKind kind, Coordinate coordinate)
        {
            return DomainAddressService.GetByCoordinate(kind, coordinate.Latitude, coordinate.Longitude);
        }

        public Task<TimeBelt> GetTimeBeltByAddress(AddressAM address)
        {
            var coordinate = address.ToCoordinate();

            return MapsService.GetTimeBelt(coordinate);
        }
    }
}
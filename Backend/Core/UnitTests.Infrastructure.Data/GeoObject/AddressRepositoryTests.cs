using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Geo;
using TransportSystems.Backend.Core.Domain.Interfaces;
using TransportSystems.Backend.Core.Domain.Interfaces.Geo;
using TransportSystems.Backend.Core.Infrastructure.Database;
using TransportSystems.Backend.Core.Infrastructure.Database.Geo;
using Xunit;

namespace TransportSystems.Backend.Core.UnitTests.Infrastructure.Data.GeoObject
{
    public class AddressRepositoryTests : BaseRepositoryTests<IAddressRepository, Address>
    {
        [Fact]
        public async Task GetProvincesWhereAddressKindIsGarage()
        {
            var entities = new []
            {
                new Address { Id = 1, Country = "Россия", Province = "Ярославская", Kind = AddressKind.Garage },
                new Address { Id = 2, Country = "Казахстан", Province = "Карагандская", Kind = AddressKind.Garage },
                new Address { Id = 3, Country = "Россия", Province = "Костромская", Kind = AddressKind.Other },
                new Address { Id = 4, Country = "Россия", Province = "Вологодская", Kind = AddressKind.Garage },
                new Address { Id = 5, Country = "Германия", Province = "Ханноверская", Kind = AddressKind.Garage },
                new Address { Id = 6, Country = "Россия", Province = "Ярославская", Kind = AddressKind.Garage }
            };

            await Repository.AddRange(entities);
            await Repository.Save();

            var result = await Repository.GetProvinces(AddressKind.Garage, "Россия");

            Assert.Equal(2, result.Count);
            Assert.Equal("Ярославская", result.ElementAt(0));
            Assert.Equal("Вологодская", result.ElementAt(1));
        }

        [Fact]
        public async Task GetLocalitiesWhereAddressKindIsGarage()
        {
            var entities = new []
            {
                new Address { Id = 1, Country = "Россия", Province = "Ярославская", Locality = "Рыбинск", Kind = AddressKind.Garage },
                new Address { Id = 2, Country = "Казахстан", Province = "Карагандская", Locality = "Караганда", Kind = AddressKind.Garage },
                new Address { Id = 3, Country = "Россия", Province = "Костромская", Locality = "Кострома", Kind = AddressKind.Other },
                new Address { Id = 4, Country = "Россия", Province = "Вологодская", Locality = "Вологда",  Kind = AddressKind.Garage },
                new Address { Id = 5, Country = "Германия", Province = "Ханноверская", Locality = "Ханнове", Kind = AddressKind.Garage },
                new Address { Id = 6, Country = "Россия", Province = "Ярославская", Locality = "Ярославль", Kind = AddressKind.Garage }
            };

            await Repository.AddRange(entities);
            await Repository.Save();

            var result = await Repository.GetLocalities(AddressKind.Garage, "Россия", "Ярославская");

            Assert.Equal(2, result.Count);
            Assert.Equal("Рыбинск", result.ElementAt(0));
            Assert.Equal("Ярославль", result.ElementAt(1));
        }

        [Fact]
        public async Task GetDistrictsWhereAddressKindIsGarage()
        {
            var entities = new []
            {
                new Address { Id = 1, Country = "Россия", Province = "Ярославская", Locality = "Рыбинск", District = "Центральный", Kind = AddressKind.Garage },
                new Address { Id = 2, Country = "Казахстан", Province = "Карагандская", Locality = "Караганда", District = "Центральный", Kind = AddressKind.Garage },
                new Address { Id = 3, Country = "Россия", Province = "Костромская", Locality = "Кострома", District = "Центральный", Kind = AddressKind.Other },
                new Address { Id = 4, Country = "Россия", Province = "Вологодская", Locality = "Вологда",  District = "Центральный", Kind = AddressKind.Garage },
                new Address { Id = 5, Country = "Германия", Province = "Ханноверская", Locality = "Ханнове", District = "Центральный", Kind = AddressKind.Garage },
                new Address { Id = 6, Country = "Россия", Province = "Ярославская", Locality = "Ярославль", District = "Центральный", Kind = AddressKind.Garage },
                new Address { Id = 7, Country = "Россия", Province = "Ярославская", Locality = "Ярославль", District = "Заволжский", Kind = AddressKind.Garage }
            };

            await Repository.AddRange(entities);
            await Repository.Save();

            var result = await Repository.GetDistricts(AddressKind.Garage, "Россия", "Ярославская", "Ярославль");

            Assert.Equal(2, result.Count);
            Assert.Equal("Центральный", result.ElementAt(0));
            Assert.Equal("Заволжский", result.ElementAt(1));
        }

        [Fact]
        public async Task GetProvincesWasOrderedByAscWhereKindIsGarage()
        {
            var entities = new []
            {
                new Address { Id = 1, Country = "Россия", Province = "Ярославская", Kind = AddressKind.Garage },
                new Address { Id = 2, Country = "Казахстан", Province = "Карагандская", Kind = AddressKind.Garage },
                new Address { Id = 3, Country = "Россия", Province = "Костромская", Kind = AddressKind.Other },
                new Address { Id = 4, Country = "Россия", Province = "Вологодская", Kind = AddressKind.Garage },
                new Address { Id = 5, Country = "Германия", Province = "Ханноверская", Kind = AddressKind.Garage },
                new Address { Id = 6, Country = "Россия", Province = "Ярославская", Kind = AddressKind.Garage },
                new Address { Id = 7, Country = "Россия", Province = "Ярославская", Kind = AddressKind.Garage },
                new Address { Id = 8, Country = "Россия", Province = "Мурманская", Kind = AddressKind.Garage }
            };

            await Repository.AddRange(entities);
            await Repository.Save();

            var result = await Repository.GetProvinces(AddressKind.Garage, "Россия", OrderingKind.Asc);

            Assert.Equal(3, result.Count);
            Assert.Equal("Вологодская", result.ElementAt(0));
            Assert.Equal("Мурманская", result.ElementAt(1));
            Assert.Equal("Ярославская", result.ElementAt(2));
        }

        [Fact]
        public async Task GetProvincesWasOrderedByDesc()
        {
            var entities = new []
            {
                new Address { Id = 1, Country = "Россия", Province = "Ярославская", Kind = AddressKind.Garage },
                new Address { Id = 2, Country = "Казахстан", Province = "Карагандская", Kind = AddressKind.Garage },
                new Address { Id = 3, Country = "Россия", Province = "Костромская", Kind = AddressKind.Other },
                new Address { Id = 4, Country = "Россия", Province = "Вологодская", Kind = AddressKind.Garage },
                new Address { Id = 5, Country = "Германия", Province = "Ханноверская", Kind = AddressKind.Garage },
                new Address { Id = 6, Country = "Россия", Province = "Ярославская", Kind = AddressKind.Garage },
                new Address { Id = 7, Country = "Россия", Province = "Ярославская", Kind = AddressKind.Garage },
                new Address { Id = 8, Country = "Россия", Province = "Мурманская", Kind = AddressKind.Garage }
            };

            await Repository.AddRange(entities);
            await Repository.Save();

            var result = await Repository.GetProvinces(AddressKind.Garage, "Россия", OrderingKind.Desc);

            Assert.Equal(3, result.Count);
            Assert.Equal("Ярославская", result.ElementAt(0));
            Assert.Equal("Мурманская", result.ElementAt(1));
            Assert.Equal("Вологодская", result.ElementAt(2));
        }

        [Fact]
        public async Task GetAddressByGeocodingWhenSearchDepthToDistrict()
        {
            var entities = new []
            {
                new Address { Id = 1, Country = "Россия", Province = "Ярославская", Locality = "Рыбинск", District = "Центральный", Street = "Кировская", Kind = AddressKind.Garage },
                new Address { Id = 2, Country = "Россия", Province = "Ярославская", Locality = "Рыбинск", District = "Центральный", Street = "Луначарского", Kind = AddressKind.Garage },
                new Address { Id = 3, Country = "Россия", Province = "Ярославская", Locality = "Рыбинск", District = "Заволжский", Street = "Волгостроевская", Kind = AddressKind.Garage },
            };

            await Repository.AddRange(entities);
            await Repository.Save();

            var result = await Repository.GetByGeocoding(AddressKind.Garage, "Россия", "Ярославская", "Рыбинск", "Центральный");

            Assert.Equal(2, result.Count);
            Assert.Equal("Россия", result.ElementAt(0).Country);
            Assert.Equal("Ярославская", result.ElementAt(0).Province);
            Assert.Equal("Рыбинск", result.ElementAt(0).Locality);
            Assert.Equal("Центральный", result.ElementAt(0).District);

            Assert.Equal("Кировская", result.ElementAt(0).Street);
            Assert.Equal("Луначарского", result.ElementAt(1).Street);
        }

        [Fact]
        public async Task GetAddressByGeocodingWhenSearchDepthToLocality()
        {
            var entities = new[]
            {
                new Address { Id = 1, Country = "Россия", Province = "Ярославская", Locality = "Рыбинск", District = "Центральный", Street = "Кировская", Kind = AddressKind.Garage },
                new Address { Id = 2, Country = "Россия", Province = "Ярославская", Locality = "Рыбинск", District = "Центральный", Street = "Луначарского", Kind = AddressKind.Garage },
                new Address { Id = 3, Country = "Россия", Province = "Ярославская", Locality = "Рыбинск", District = "Заволжский", Street = "Волгостроевская", Kind = AddressKind.Garage },
            };

            await Repository.AddRange(entities);
            await Repository.Save();

            var result = await Repository.GetByGeocoding(AddressKind.Garage, "Россия", "Ярославская", "Рыбинск");

            Assert.Equal(3, result.Count);
            Assert.Equal("Россия", result.ElementAt(0).Country);
            Assert.Equal("Ярославская", result.ElementAt(0).Province);
            Assert.Equal("Рыбинск", result.ElementAt(0).Locality);

            Assert.Equal("Центральный", result.ElementAt(0).District);
            Assert.Equal("Центральный", result.ElementAt(1).District);
            Assert.Equal("Заволжский", result.ElementAt(2).District);
        }

        [Fact]
        public async Task GetAddressesInCoordinateBounds()
        {
            var entities = new[]
            {
                new Address { Id = 1, Kind = AddressKind.Garage, Latitude = 1, Longitude = 3 },
                new Address { Id = 2, Kind = AddressKind.Garage, Latitude = -3, Longitude = -4 },
                new Address { Id = 3, Kind = AddressKind.Vehicle, Latitude = 1, Longitude = 2 },
                new Address { Id = 4, Kind = AddressKind.Vehicle, Latitude = 1, Longitude = 2 },
                new Address { Id = 5, Kind = AddressKind.Garage, Latitude = 1.6, Longitude = 2 },
                new Address { Id = 6, Kind = AddressKind.Garage, Latitude = 1.01, Longitude = 2.1 },
            };

            await Repository.AddRange(entities);
            await Repository.Save();

            var result = await Repository.GetInCoordinateBounds(AddressKind.Garage, 1d, 2d, 1.5d, 3.5d);

            Assert.Equal(2, result.Count);
            Assert.Equal(1, result.ElementAt(0).Id);
            Assert.Equal(6, result.ElementAt(1).Id);
        }

        [Fact]
        public async Task GetAddressByCoordinate()
        {
            var entities = new[]
            {
                new Address { Id = 1, Kind = AddressKind.Garage, Latitude = 1, Longitude = 3 },
                new Address { Id = 2, Kind = AddressKind.Garage, Latitude = -3, Longitude = -4 },
                new Address { Id = 3, Kind = AddressKind.Vehicle, Latitude = 1, Longitude = 2 },
                new Address { Id = 4, Kind = AddressKind.Vehicle, Latitude = 1, Longitude = 2 },
                new Address { Id = 5, Kind = AddressKind.Garage, Latitude = 1.6, Longitude = 2 },
                new Address { Id = 6, Kind = AddressKind.Garage, Latitude = 1.01, Longitude = 2.1 },
            };

            await Repository.AddRange(entities);
            await Repository.Save();

            var result = await Repository.GetByCoordinate(1.6, 2);

            Assert.Equal(5, result.Id);
        }

        protected override IAddressRepository CreateRepository(ApplicationContext context)
        {
            return new AddressRepository(context);
        }
    }
}
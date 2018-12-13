using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TransportSystems.Backend.Application.Interfaces.Catalogs;
using TransportSystems.Backend.Application.Interfaces.Organization;
using TransportSystems.Backend.Application.Models.Catalogs;
using TransportSystems.Backend.Application.Models.Geo;
using TransportSystems.Backend.Core.Domain.Core.Catalogs;
using TransportSystems.Backend.Core.Domain.Core.Transport;
using TransportSystems.Backend.Core.Infrastructure.Database;
using TransportSystems.Backend.Core.Services.Interfaces.Catalogs;

namespace TransportSystems.Backend.API.Database
{
    public static class DbInitializator
    {
        public static void Initialize(IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var services = scope.ServiceProvider;
                services.GetService<ApplicationContext>().Database.Migrate();

                CatalogService = services.GetRequiredService<ICatalogService>();

                ApplicationCatalogService = services.GetRequiredService<IApplicationCatalogService>(); 
                CityService = services.GetRequiredService<IApplicationCityService>();
                GarageService = services.GetRequiredService<IApplicationGarageService>();              

                InitCatalogs().GetAwaiter().GetResult();
                InitOrganizations().GetAwaiter().GetResult();
            }
        }

        public static ICatalogService CatalogService { get; private set; }

        public static IApplicationCatalogService ApplicationCatalogService { get; private set; }

        public static IApplicationCityService CityService { get; private set; }

        public static IApplicationGarageService GarageService { get; private set; }

        #region Organization
        private static async Task InitOrganizations()
        {
            var rybinskDomain = "rybinsk";
            var rybinskAddress = new AddressAM { Country = "Россия", Province = "Ярославская", Area = "Рыбинский район", Locality = "Рыбинск", Latitude = 58.0610321, Longitude = 38.7416854 };
            var rybinskGaragesAddresses = new List<AddressAM>
            {
                new AddressAM { Country = "Россия", Province = "Ярославская", Area = "Рыбинский район", Locality = "Рыбинск", District = "Центральный", Street = "Пр. Серова", House = "1", Latitude = 58.0569028, Longitude = 38.780219 },
                new AddressAM { Country = "Россия", Province = "Ярославская", Area = "Рыбинский район", Locality = "Рыбинск", District = "Заволжский", Street = "Большая Вольская", House = "55", Latitude = 58.0681517, Longitude = 38.8615214 }
            };
            await CreateCityInfrastructure(rybinskDomain, rybinskAddress, rybinskGaragesAddresses);

            var vologdaDomain = "vologda";
            var vologdaAddress = new AddressAM { Country = "Россия", Province = "Вологодская", Locality = "Вологда", Latitude = 59.2221979, Longitude = 39.8057537 };
            var vologdaGaragesAddresses = new List<AddressAM>
            {
                new AddressAM { Country = "Россия", Province = "Вологодская", Locality = "Вологда", District = "Центральный", Street = "Благовещенская", House = "20", Latitude = 59.2224107, Longitude = 39.8715978 }
            };
            await CreateCityInfrastructure(vologdaDomain, vologdaAddress, vologdaGaragesAddresses);

            var cherepovecDomain = "cherepovec";
            var cherepovecAddress = new AddressAM { Country = "Россия", Province = "Вологодская", Locality = "Череповец", Latitude = 59.1291174, Longitude = 37.7098701 };
            var cherepovecGaragesAddresses = new List<AddressAM>
            {
                new AddressAM { Country = "Россия", Province = "Вологодская", Locality = "Череповец", District = "Центральный", Street = "Сталеваров", House = "78", Latitude = 59.1367202, Longitude = 37.9103716 }
            };
            await CreateCityInfrastructure(cherepovecDomain, cherepovecAddress, cherepovecGaragesAddresses);

            var yaroslavlDomain = "yaroslavl";
            var yaroslavlAddress = new AddressAM { Country = "Россия", Province = "Ярославская", Locality = "Ярославль", Latitude = 57.6525644, Longitude = 39.724092 };
            var yaroslavlGaragesAddresses = new List<AddressAM>
            {
                new AddressAM { Country = "Россия", Province = "Ярославская", Locality = "Ярославль", District = "Ленинский", Street = "Радищева", House = "24", Latitude = 57.6379029, Longitude = 39.840627 },
            };
            await CreateCityInfrastructure(yaroslavlDomain, yaroslavlAddress, yaroslavlGaragesAddresses);
        }

        private static async Task CreateCityInfrastructure(
            string cityDomain,
            AddressAM cityAddress,
            List<AddressAM> garagesAddresses)
        {
            if (!await CityService.IsExistByDomain(cityDomain))
            {
                var domainCity = await CityService.CreateDomainCity(cityDomain, cityAddress);

                foreach (var garageAddress in garagesAddresses)
                {
                    await GarageService.CreateDomainGarage(domainCity.Id, garageAddress);
                }
            }
        }
        #endregion

        #region Catalog
        private static async Task InitCatalogs()
        {
            var kinds = Enum.GetValues(typeof(CatalogKind));
            foreach (var kind in kinds)
            {
                var catalogKind = (CatalogKind)kind;

                var catalog = await CatalogService.GetByKind(catalogKind);
                if (catalog == null)
                {
                    catalog = await CatalogService.Create(catalogKind);
                    switch(catalogKind)
                    {
                        case CatalogKind.Cargo:
                            {
                                await InitCargoCatalogItems(catalog.Id);
                                break;
                            }
                        case CatalogKind.Vehicle:
                            {
                                await InitVehicleCatalogItems(catalog.Id);
                                break;
                            }
                    }
                }
            }
        }

        private static async Task CreateCatalogItems(
            int catalogId,
            List<CatalogItemAM> items)
        {
            foreach(var item in items)
            {
                await ApplicationCatalogService.CreateCatalogItem(catalogId, item);
            }
        }

        private static async Task InitVehicleCatalogItems(int catalogId)
        {
            var catalogItems = new List<CatalogItemAM>
            {
                new CatalogItemAM { Kind = CatalogItemKind.Brand, Name = "Мерседес", Value = 0 },
                new CatalogItemAM { Kind = CatalogItemKind.Brand, Name = "Тойота", Value = 0 },
                new CatalogItemAM { Kind = CatalogItemKind.Brand, Name = "Газель", Value = 0 },

                new CatalogItemAM { Kind = CatalogItemKind.Capacity, Name = "1 т", Value = 1000 },
                new CatalogItemAM { Kind = CatalogItemKind.Capacity, Name = "2 т", Value = 2000 },
                new CatalogItemAM { Kind = CatalogItemKind.Capacity, Name = "2.5 т", Value = 2500 },

                new CatalogItemAM { Kind = CatalogItemKind.Kind, Name = "Ломаная платформа", Value = (int)VehicleKind.BrokenPlatform },
                new CatalogItemAM { Kind = CatalogItemKind.Kind, Name = "Сдвижная платформа", Value = (int)VehicleKind.SlidingPlatform },
                new CatalogItemAM { Kind = CatalogItemKind.Kind, Name = "Погрузчик", Value = (int)VehicleKind.CraneManipulator }
            };

            await CreateCatalogItems(catalogId, catalogItems);
        }

        private static async Task InitCargoCatalogItems(int catalogId)
        {
            var catalogItems = new List<CatalogItemAM>
            {
                new CatalogItemAM { Kind = CatalogItemKind.Brand, Name = "Мерседес", Value = 0 },
                new CatalogItemAM { Kind = CatalogItemKind.Brand, Name = "Тойота", Value = 0 },
                new CatalogItemAM { Kind = CatalogItemKind.Brand, Name = "Ниссан", Value = 0 },
                new CatalogItemAM { Kind = CatalogItemKind.Brand, Name = "ВАЗ", Value = 0 },
                new CatalogItemAM { Kind = CatalogItemKind.Brand, Name = "Бентли", Value = 0 },

                new CatalogItemAM { Kind = CatalogItemKind.Weight, Name = "до 0.5 т", Value = 500 },
                new CatalogItemAM { Kind = CatalogItemKind.Weight, Name = "до 1.5 т", Value = 1500 },
                new CatalogItemAM { Kind = CatalogItemKind.Weight, Name = "до 2.0 т", Value = 2000 },
                new CatalogItemAM { Kind = CatalogItemKind.Weight, Name = "до 2.5 т", Value = 2500 },

                new CatalogItemAM { Kind = CatalogItemKind.Kind, Name = "Мототехника", Value = (int)CargoKind.Bike },
                new CatalogItemAM { Kind = CatalogItemKind.Kind, Name = "Легковая машина", Value = (int)CargoKind.Car },
                new CatalogItemAM { Kind = CatalogItemKind.Kind, Name = "Кроссовер", Value = (int)CargoKind.Crossover },
                new CatalogItemAM { Kind = CatalogItemKind.Kind, Name = "Внедорожник", Value = (int)CargoKind.SUV }
            };

            await CreateCatalogItems(catalogId, catalogItems);
        }
        #endregion
    }
}
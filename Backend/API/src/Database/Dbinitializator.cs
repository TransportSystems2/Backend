using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TransportSystems.Backend.Application.Business.Geo;
using TransportSystems.Backend.Application.Interfaces;
using TransportSystems.Backend.Application.Interfaces.Catalogs;
using TransportSystems.Backend.Application.Interfaces.Organization;
using TransportSystems.Backend.Application.Models.Catalogs;
using TransportSystems.Backend.Application.Models.Geo;
using TransportSystems.Backend.Application.Models.SignUp;
using TransportSystems.Backend.Application.Models.Transport;
using TransportSystems.Backend.Application.Models.Users;
using TransportSystems.Backend.Core.Domain.Core.Catalogs;
using TransportSystems.Backend.Core.Domain.Core.Organization;
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

                DomainCatalogService = services.GetRequiredService<ICatalogService>();

                CatalogService = services.GetRequiredService<IApplicationCatalogService>(); 
                GarageService = services.GetRequiredService<IApplicationGarageService>();
                MarketService = services.GetRequiredService<IApplicationMarketService>();
                CompanyService = services.GetRequiredService<IApplicationCompanyService>();
                SignUpService = services.GetRequiredService<ISignUpService>();

                InitCatalogs().GetAwaiter().GetResult();
                InitOrganizations().GetAwaiter().GetResult();
            }
        }

        public static ICatalogService DomainCatalogService { get; private set; }

        public static IApplicationCatalogService CatalogService { get; private set; }

        public static IApplicationGarageService GarageService { get; private set; }

        public static IApplicationMarketService MarketService { get; private set; }

        public static IApplicationCompanyService CompanyService { get; private set; } 

        public static ISignUpService SignUpService { get; private set; } 

        private static async Task<Company> InitCompany(string companyName)
        {
            var domainCompany = await CompanyService.GetDomainCompany(companyName);
            if (domainCompany == null)
            {
                var vehicle = new VehicleAM
                {
                    RegistrationNumber = "К100ЕЕ77",
                    BrandCatalogItemId = 3,
                    CapacityCatalogItemId = 6,
                    KindCatalogItemId = 7
                };

                var dispatcher = new DispatcherAM
                {
                    FirstName = "Павел",
                    LastName = "Федоров",
                    PhoneNumber = "79159882658"
                };

                var driver = new DriverAM
                {
                    FirstName = "Артур",
                    LastName = "Пирожков",
                    PhoneNumber = "78912378123"
                };

                var garageAddress = new AddressAM();

                var companyApplication = new CompanyApplicationAM
                {
                    Dispatcher = dispatcher,
                    GarageAddress = garageAddress,
                    Vehicle = vehicle,
                    Driver = driver
                };

                await SignUpService.SignUpCompany(companyApplication);
            }

            domainCompany = await CompanyService.GetDomainCompany(companyName);

            return domainCompany;
        }

        #region Organization
        private static async Task InitOrganizations()
        {
            var domainCompany = await InitCompany("79159882658");

            var marketAddresses = new List<AddressAM>
            {
                new AddressAM { Country = "Россия", Province = "Ярославская", Area = "Рыбинский район", Locality = "Рыбинск", District = "Центральный", Street = "Пр. Серова", House = "1", Latitude = 58.0569028, Longitude = 38.780219 },
                new AddressAM { Country = "Россия", Province = "Вологодская", Locality = "Вологда", District = "Центральный", Street = "Благовещенская", House = "20", Latitude = 59.2224107, Longitude = 39.8715978 },
                new AddressAM { Country = "Россия", Province = "Вологодская", Locality = "Череповец", District = "Центральный", Street = "Сталеваров", House = "78", Latitude = 59.1367202, Longitude = 37.9103716 },
                new AddressAM { Country = "Россия", Province = "Ярославская", Locality = "Ярославль", District = "Ленинский", Street = "Радищева", House = "24", Latitude = 57.6379029, Longitude = 39.840627 }
            };

            await InitMarkets(domainCompany.Id, marketAddresses);
        }

        private static async Task InitMarkets(
            int companyId,
            List<AddressAM> marketAddresses)
        {
            foreach (var marketAddress in marketAddresses)
            {
                if (await MarketService.GetDomainMarketByCoordinate(marketAddress.ToCoordinate()) == null)
                {
                    await MarketService.CreateDomainMarket(companyId, marketAddress);
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

                var catalog = await DomainCatalogService.GetByKind(catalogKind);
                if (catalog == null)
                {
                    catalog = await DomainCatalogService.Create(catalogKind);
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
                await CatalogService.CreateCatalogItem(catalogId, item);
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
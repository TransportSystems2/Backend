using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.IO;
using System.Reflection;
using TransportSystems.Backend.API.Database;
using TransportSystems.Backend.API.Mapping;
using TransportSystems.Backend.Application.Business;
using TransportSystems.Backend.Application.Business.Billing;
using TransportSystems.Backend.Application.Business.Booking;
using TransportSystems.Backend.Application.Business.Catalogs;
using TransportSystems.Backend.Application.Business.Geo;
using TransportSystems.Backend.Application.Business.Mapping;
using TransportSystems.Backend.Application.Business.Ordering;
using TransportSystems.Backend.Application.Business.Organization;
using TransportSystems.Backend.Application.Business.Pricing;
using TransportSystems.Backend.Application.Business.Prognosing;
using TransportSystems.Backend.Application.Interfaces;
using TransportSystems.Backend.Application.Interfaces.Billing;
using TransportSystems.Backend.Application.Interfaces.Booking;
using TransportSystems.Backend.Application.Interfaces.Catalogs;
using TransportSystems.Backend.Application.Interfaces.Geo;
using TransportSystems.Backend.Application.Interfaces.Mapping;
using TransportSystems.Backend.Application.Interfaces.Ordering;
using TransportSystems.Backend.Application.Interfaces.Organization;
using TransportSystems.Backend.Application.Interfaces.Pricing;
using TransportSystems.Backend.Application.Interfaces.Prognosing;
using TransportSystems.Backend.Application.Interfaces.Routing;
using TransportSystems.Backend.Application.Interfaces.Users;
using TransportSystems.Backend.Core.Domain.Interfaces;
using TransportSystems.Backend.Core.Domain.Interfaces.Billing;
using TransportSystems.Backend.Core.Domain.Interfaces.Catalogs;
using TransportSystems.Backend.Core.Domain.Interfaces.Geo;
using TransportSystems.Backend.Core.Domain.Interfaces.Ordering;
using TransportSystems.Backend.Core.Domain.Interfaces.Organization;
using TransportSystems.Backend.Core.Domain.Interfaces.Pricing;
using TransportSystems.Backend.Core.Domain.Interfaces.Routing;
using TransportSystems.Backend.Core.Domain.Interfaces.Trading;
using TransportSystems.Backend.Core.Domain.Interfaces.Transport;
using TransportSystems.Backend.Core.Domain.Interfaces.Users;
using TransportSystems.Backend.Core.Infrastructure.Business;
using TransportSystems.Backend.Core.Infrastructure.Business.Billing;
using TransportSystems.Backend.Core.Infrastructure.Business.Catalogs;
using TransportSystems.Backend.Core.Infrastructure.Business.Geo;
using TransportSystems.Backend.Core.Infrastructure.Business.Notification;
using TransportSystems.Backend.Core.Infrastructure.Business.Organization;
using TransportSystems.Backend.Core.Infrastructure.Business.Pricing;
using TransportSystems.Backend.Core.Infrastructure.Business.RegistrationNumber;
using TransportSystems.Backend.Core.Infrastructure.Business.Routing;
using TransportSystems.Backend.Core.Infrastructure.Business.Trading;
using TransportSystems.Backend.Core.Infrastructure.Business.Transport;
using TransportSystems.Backend.Core.Infrastructure.Business.Users;
using TransportSystems.Backend.Core.Infrastructure.Database;
using TransportSystems.Backend.Core.Infrastructure.Database.Billing;
using TransportSystems.Backend.Core.Infrastructure.Database.Catalogs;
using TransportSystems.Backend.Core.Infrastructure.Database.Geo;
using TransportSystems.Backend.Core.Infrastructure.Database.Ordering;
using TransportSystems.Backend.Core.Infrastructure.Database.Organization;
using TransportSystems.Backend.Core.Infrastructure.Database.Pricing;
using TransportSystems.Backend.Core.Infrastructure.Database.Routing;
using TransportSystems.Backend.Core.Infrastructure.Database.Trading;
using TransportSystems.Backend.Core.Infrastructure.Database.Users;
using TransportSystems.Backend.Core.Infrastructure.Http.Users;
using TransportSystems.Backend.Core.Services.Interfaces;
using TransportSystems.Backend.Core.Services.Interfaces.Billing;
using TransportSystems.Backend.Core.Services.Interfaces.Catalogs;
using TransportSystems.Backend.Core.Services.Interfaces.Geo;
using TransportSystems.Backend.Core.Services.Interfaces.Interfaces;
using TransportSystems.Backend.Core.Services.Interfaces.Notification;
using TransportSystems.Backend.Core.Services.Interfaces.Ordering;
using TransportSystems.Backend.Core.Services.Interfaces.Organization;
using TransportSystems.Backend.Core.Services.Interfaces.Pricing;
using TransportSystems.Backend.Core.Services.Interfaces.RegistrationNumber;
using TransportSystems.Backend.Core.Services.Interfaces.Routing;
using TransportSystems.Backend.Core.Services.Interfaces.Trading;
using TransportSystems.Backend.Core.Services.Interfaces.Transport;
using TransportSystems.Backend.Core.Services.Interfaces.Users;
using TransportSystems.Backend.External.Business.Direction;
using TransportSystems.Backend.External.Business.Direction.Providers;
using TransportSystems.Backend.External.Business.Geocoder;
using TransportSystems.Backend.External.Business.Geocoder.Providers;
using TransportSystems.Backend.External.Business.Maps;
using TransportSystems.Backend.External.Business.Maps.Providers;
using TransportSystems.Backend.External.Interfaces.Direction;
using TransportSystems.Backend.External.Interfaces.Geocoder;
using TransportSystems.Backend.External.Interfaces.Maps;
using TransportSystems.Backend.External.Interfaces.Services.Direction;
using TransportSystems.Backend.External.Interfaces.Services.Geocoder;
using TransportSystems.Backend.External.Interfaces.Services.Maps;
using TransportSystems.Backend.External.Models.Enums;
using TransportSystems.Backend.Identity.API;

namespace TransportSystems.Backend.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            ConfigureSwagger(services);
            ConfigureMapper(services);
            ConfigureAuthentication(services);
            ConfigureApplicationContext(services);
            ConfigureCustomRepositories(services);
            ConfigureCustomServices(services);
            ConfigureApplications(services);
            ConfigureExternalServices(services);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            DbInitializator.Initialize(app);

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "TrasnportSystems API V1");
                c.RoutePrefix = string.Empty;
            });

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();

            app.UseMvc();
        }

        protected virtual void ConfigureSwagger(IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "TransportSystems API", Version = "v1" });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
        }

        protected virtual void ConfigureMapper(IServiceCollection services)
        {
            services.AddAutoMapper();

            Mapper.Initialize(cfg =>
            {
                cfg.AddProfile<UsersProfile>();
            });
        }

        protected virtual void ConfigureAuthentication(IServiceCollection services)
        {
            services.AddAuthorization();
            services.AddAuthentication("Bearer")
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = Configuration.GetConnectionString("identity");
                    options.RequireHttpsMetadata = false;

                    options.ApiName = "TSAPI";
                });
        }

        protected virtual void ConfigureApplicationContext(IServiceCollection services)
        {
            services.AddDbContext<ApplicationContext>(
                options => options.UseNpgsql(Configuration.GetConnectionString("db"),
                b => b.MigrationsAssembly("TransportSystems.Backend.API")));
        }

        protected virtual void ConfigureCustomRepositories(IServiceCollection services)
        {
            services.AddScoped<IContext>(
                provider => provider.GetService<ApplicationContext>()
            );

            services.AddScoped<IIdentityUserRepository, IdentityUserRepository>();
            services.AddScoped<IModeratorRepository, ModeratorRepository>();
            services.AddScoped<IDriverRepository, DriverRepository>();
            services.AddScoped<IDispatcherRepository, DispatcherRepository>();

            services.AddScoped<IVehicleRepository, VehicleRepository>();
            services.AddScoped<IAddressRepository, AddressRepository>();
            services.AddScoped<ICompanyRepository, CompanyRepository>();
            services.AddScoped<ICityRepository, CityRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IOrderStateRepository, OrderStateRepository>();
            services.AddScoped<ILotRepository, LotRepository>();
            services.AddScoped<ILotRequestRepository, LotRequestRepository>();
            services.AddScoped<IGarageRepository, GarageRepository>();
            services.AddScoped<ICargoRepository, CargoRepository>();
            services.AddScoped<IRouteRepository, RouteRepository>();
            services.AddScoped<IRouteLegRepository, RouteLegRepository>();
            services.AddScoped<ICatalogItemRepository, CatalogItemRepository>();
            services.AddScoped<ICatalogRepository, CatalogRepository>();
            services.AddScoped<IPricelistRepository, PricelistRepository>();
            services.AddScoped<IPriceRepository, PriceRepository>();
            services.AddScoped<IBillRepository, BillRepository>();
            services.AddScoped<IBillItemRepository, BillItemRepository>();
            services.AddScoped<IBasketRepository, BasketRepository>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();
        }

        protected virtual void ConfigureCustomServices(IServiceCollection services)
        {
            services.AddScoped<ITransactionService, TransactionService>();

            services.AddScoped<IIdentityUserService, IdentityUserService>();
            services.AddScoped<IModeratorService, ModeratorService>();
            services.AddScoped<IDriverService, DriverService>();
            services.AddScoped<IDispatcherService, DispatcherService>();

            services.AddScoped<IVehicleService, VehicleService>();

            services.AddScoped<IAddressService, AddressService>();
            services.AddScoped<ICompanyService, CompanyService>();
            services.AddScoped<ICityService, CityService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IOrderStateService, OrderStateService>();
            services.AddScoped<ILotService, LotService>();
            services.AddScoped<ILotRequestService, LotRequestService>();
            services.AddScoped<IGarageService, GarageService>();
            services.AddScoped<ICargoService, CargoService>();
            services.AddScoped<IRouteService, RouteService>();
            services.AddScoped<IRouteLegService, RouteLegService>();
            services.AddScoped<ICatalogItemService, CatalogItemService>();
            services.AddScoped<ICatalogService, CatalogService>();
            services.AddScoped<IRegistrationNumberService, RegistrationNumberService>();
            services.AddScoped<IPricelistService, PricelistService>();
            services.AddScoped<IPriceService, PriceService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IBillService, BillService>();
            services.AddScoped<IBillItemService, BillItemService>();
            services.AddScoped<IBasketService, BasketService>();

            services.AddScoped<ICustomerService, CustomerService>();

            var identityUri = Configuration.GetConnectionString("identity_manage");
            var identityUsersAPI = IdentityUsersAPIFactory<IIdentityUsersAPI>.Create(identityUri);
            services.AddSingleton(identityUsersAPI);
        }

        protected virtual void ConfigureApplications(IServiceCollection services)
        {
            services.AddScoped<IMappingService, ApplicationMappingService>();
            services.AddScoped<ISignUpService, SignUpService>();
            services.AddScoped<IApplicationGarageService, ApplicationGarageService>();
            services.AddScoped<IApplicationVehicleService, ApplicationVehicleService>();
            services.AddScoped<IApplicationCatalogService, ApplicationCatalogService>();
            services.AddScoped<IApplicationCargoService, ApplicationCargoService>();
            services.AddScoped<IApplicationAddressService, ApplicationAddressService>();
            services.AddScoped<IApplicationBillService, ApplicationBillService>();
            services.AddScoped<IApplicationBookingService, ApplicationBookingService>();
            services.AddScoped<IApplicationPricelistService, ApplicationPricelistService>();
            services.AddScoped<IApplicationCityService, ApplicationCityService>();
            services.AddScoped<IApplicationRouteService, ApplicationRouteService>();
            services.AddScoped<IApplicationOrderService, ApplicationOrderService>();
            services.AddScoped<IApplicationOrderValidatorService, ApplicationOrderValidatorService>();
            services.AddScoped<IApplicationCustomerService, ApplicationCustomerService>();
            services.AddScoped<IApplicationPrognosisService, ApplicationPrognosisService>();
        }

        protected virtual void ConfigureExternalServices(IServiceCollection services)
        {
            services.AddScoped<IDirectionService, DirectionService>();
            services.AddScoped<IDirectionAccessor>(sp => new DirectionAccessor(GetDirectionProvider));

            services.AddScoped<IGeocoderService, GeocoderService>();
            services.AddScoped<IGeocoderAccessor>(sp => new GeocoderAccessor(GetGeocoderProvider));

            services.AddScoped<IMapsService, MapsService>();
            services.AddScoped<IMapsAccessor>(sp => new MapsAccessor(GetMapsProvider));
        }

        protected IDirection GetDirectionProvider(ProviderKind kind)
        {
            switch (kind)
            {
                case ProviderKind.Google:
                    {
                        return new GoogleDirection();
                    }
            }

            return new GoogleDirection();
        }

        protected IGeocoder GetGeocoderProvider(ProviderKind kind)
        {
            switch (kind)
            {
                case ProviderKind.Google:
                    {
                        return new GoogleGeocoder();
                    }

                case ProviderKind.Yandex:
                    {
                        return new YandexGeocoder();
                    }

                case ProviderKind.DaData:
                    {
                        return new DaDataGeocoder();
                    }
            }

            return new GoogleGeocoder();
        }

        protected IMaps GetMapsProvider(ProviderKind kind)
        {
            switch (kind)
            {
                case ProviderKind.Google:
                    {
                        return new GoogleMaps();
                    }
            }

            return new GoogleMaps();
        }
    }
}
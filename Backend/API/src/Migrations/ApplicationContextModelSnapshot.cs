﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using TransportSystems.Backend.Core.Infrastructure.Database;

namespace TransportSystems.Backend.API.Migrations
{
    [DbContext(typeof(ApplicationContext))]
    partial class ApplicationContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "2.2.0-rtm-35687")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("TransportSystems.Backend.Core.Domain.Core.Billing.Basket", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("AddedDate");

                    b.Property<int>("DitchValue");

                    b.Property<int>("LoadingValue");

                    b.Property<int>("LockedSteeringValue");

                    b.Property<int>("LockedWheelsValue");

                    b.Property<DateTime>("ModifiedDate");

                    b.Property<int>("OverturnedValue");

                    b.HasKey("Id");

                    b.ToTable("Baskets");
                });

            modelBuilder.Entity("TransportSystems.Backend.Core.Domain.Core.Billing.Bill", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("AddedDate");

                    b.Property<int>("BasketId");

                    b.Property<byte>("CommissionPercentage");

                    b.Property<float>("DegreeOfDifficulty");

                    b.Property<DateTime>("ModifiedDate");

                    b.Property<int>("PriceId");

                    b.Property<decimal>("TotalCost");

                    b.HasKey("Id");

                    b.ToTable("Bills");
                });

            modelBuilder.Entity("TransportSystems.Backend.Core.Domain.Core.Billing.BillItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("AddedDate");

                    b.Property<int>("BillId");

                    b.Property<decimal>("Cost");

                    b.Property<string>("Key");

                    b.Property<DateTime>("ModifiedDate");

                    b.Property<decimal>("Price");

                    b.Property<int>("Value");

                    b.HasKey("Id");

                    b.ToTable("BillItems");
                });

            modelBuilder.Entity("TransportSystems.Backend.Core.Domain.Core.Catalogs.Catalog", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("AddedDate");

                    b.Property<int>("Kind");

                    b.Property<DateTime>("ModifiedDate");

                    b.HasKey("Id");

                    b.ToTable("Catalogs");
                });

            modelBuilder.Entity("TransportSystems.Backend.Core.Domain.Core.Catalogs.CatalogItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("AddedDate");

                    b.Property<int>("CatalogId");

                    b.Property<int>("Kind");

                    b.Property<DateTime>("ModifiedDate");

                    b.Property<string>("Name");

                    b.Property<int>("Value");

                    b.HasKey("Id");

                    b.ToTable("CatalogItems");
                });

            modelBuilder.Entity("TransportSystems.Backend.Core.Domain.Core.Geo.Address", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("AddedDate");

                    b.Property<double>("AdjustedLatitude");

                    b.Property<double>("AdjustedLongitude");

                    b.Property<string>("Area");

                    b.Property<string>("Country");

                    b.Property<string>("District");

                    b.Property<string>("FormattedText");

                    b.Property<string>("House");

                    b.Property<int>("Kind");

                    b.Property<double>("Latitude");

                    b.Property<string>("Locality");

                    b.Property<double>("Longitude");

                    b.Property<DateTime>("ModifiedDate");

                    b.Property<string>("Province");

                    b.Property<string>("Request");

                    b.Property<string>("Street");

                    b.HasKey("Id");

                    b.ToTable("Address");
                });

            modelBuilder.Entity("TransportSystems.Backend.Core.Domain.Core.Ordering.Order", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("AddedDate");

                    b.Property<DateTime>("ModifiedDate");

                    b.HasKey("Id");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("TransportSystems.Backend.Core.Domain.Core.Ordering.OrderState", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("AddedDate");

                    b.Property<int>("BillId");

                    b.Property<int>("CargoId");

                    b.Property<int>("CustomerId");

                    b.Property<int>("DriverId");

                    b.Property<int>("GarageId");

                    b.Property<int>("GenCompanyId");

                    b.Property<int>("GenDispatcherId");

                    b.Property<int>("MarketId");

                    b.Property<DateTime>("ModifiedDate");

                    b.Property<int>("OrderId");

                    b.Property<int>("PathId");

                    b.Property<int>("RouteId");

                    b.Property<int>("Status");

                    b.Property<int>("SubCompanyId");

                    b.Property<int>("SubDispatcherId");

                    b.Property<DateTime>("TimeOfDelivery");

                    b.Property<int>("VehicleId");

                    b.HasKey("Id");

                    b.ToTable("OrderStates");
                });

            modelBuilder.Entity("TransportSystems.Backend.Core.Domain.Core.Organization.Company", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("AddedDate");

                    b.Property<DateTime>("ModifiedDate");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Companies");
                });

            modelBuilder.Entity("TransportSystems.Backend.Core.Domain.Core.Organization.Garage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("AddedDate");

                    b.Property<int>("AddressId");

                    b.Property<int>("CompanyId");

                    b.Property<DateTime>("ModifiedDate");

                    b.HasKey("Id");

                    b.ToTable("Garages");
                });

            modelBuilder.Entity("TransportSystems.Backend.Core.Domain.Core.Organization.Market", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("AddedDate");

                    b.Property<int>("AddressId");

                    b.Property<int>("CompanyId");

                    b.Property<DateTime>("ModifiedDate");

                    b.Property<int>("PricelistId");

                    b.HasKey("Id");

                    b.ToTable("Markets");
                });

            modelBuilder.Entity("TransportSystems.Backend.Core.Domain.Core.Pricing.Price", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("AddedDate");

                    b.Property<int>("CatalogItemId");

                    b.Property<byte>("CommissionPercentage");

                    b.Property<decimal>("Ditch");

                    b.Property<decimal>("Loading");

                    b.Property<decimal>("LockedSteering");

                    b.Property<decimal>("LockedWheel");

                    b.Property<DateTime>("ModifiedDate");

                    b.Property<string>("Name");

                    b.Property<decimal>("Overturned");

                    b.Property<decimal>("PerMeter");

                    b.Property<int>("PricelistId");

                    b.HasKey("Id");

                    b.ToTable("Prices");
                });

            modelBuilder.Entity("TransportSystems.Backend.Core.Domain.Core.Pricing.Pricelist", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("AddedDate");

                    b.Property<DateTime>("ModifiedDate");

                    b.HasKey("Id");

                    b.ToTable("Pricelists");
                });

            modelBuilder.Entity("TransportSystems.Backend.Core.Domain.Core.Routing.Route", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("AddedDate");

                    b.Property<string>("Comment");

                    b.Property<DateTime>("ModifiedDate");

                    b.HasKey("Id");

                    b.ToTable("Routes");
                });

            modelBuilder.Entity("TransportSystems.Backend.Core.Domain.Core.Routing.RouteLeg", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("AddedDate");

                    b.Property<TimeSpan>("Duration");

                    b.Property<int>("EndAddressId");

                    b.Property<int>("Kind");

                    b.Property<DateTime>("ModifiedDate");

                    b.Property<int>("RouteId");

                    b.Property<int>("StartAddressId");

                    b.HasKey("Id");

                    b.ToTable("RouteLegs");
                });

            modelBuilder.Entity("TransportSystems.Backend.Core.Domain.Core.Routing.Track", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("AddedDate");

                    b.Property<DateTime>("ModifiedDate");

                    b.HasKey("Id");

                    b.ToTable("Paths");
                });

            modelBuilder.Entity("TransportSystems.Backend.Core.Domain.Core.Routing.TrackPoint", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("AddedDate");

                    b.Property<double>("Latitude");

                    b.Property<double>("Longitude");

                    b.Property<DateTime>("ModifiedDate");

                    b.Property<byte>("Speed");

                    b.Property<int>("Timestamp");

                    b.Property<int>("TrackId");

                    b.HasKey("Id");

                    b.ToTable("PathPoints");
                });

            modelBuilder.Entity("TransportSystems.Backend.Core.Domain.Core.Trading.Lot", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("AddedDate");

                    b.Property<DateTime>("ModifiedDate");

                    b.Property<int>("OrderId");

                    b.Property<int>("Status");

                    b.Property<int>("WinnerDispatcherId");

                    b.HasKey("Id");

                    b.ToTable("Lots");
                });

            modelBuilder.Entity("TransportSystems.Backend.Core.Domain.Core.Trading.LotRequest", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("AddedDate");

                    b.Property<int>("DispatcherId");

                    b.Property<int>("LotId");

                    b.Property<DateTime>("ModifiedDate");

                    b.Property<int>("Status");

                    b.HasKey("Id");

                    b.ToTable("LotRequests");
                });

            modelBuilder.Entity("TransportSystems.Backend.Core.Domain.Core.Transport.Cargo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("AddedDate");

                    b.Property<int>("BrandCatalogItemId");

                    b.Property<string>("Comment");

                    b.Property<int>("KindCatalogItemId");

                    b.Property<DateTime>("ModifiedDate");

                    b.Property<string>("RegistrationNumber");

                    b.Property<int>("WeightCatalogItemId");

                    b.HasKey("Id");

                    b.ToTable("Cargoes");
                });

            modelBuilder.Entity("TransportSystems.Backend.Core.Domain.Core.Transport.Vehicle", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("AddedDate");

                    b.Property<int>("BrandCatalogItemId");

                    b.Property<int>("CapacityCatalogItemId");

                    b.Property<int>("CompanyId");

                    b.Property<int>("KindCatalogItemId");

                    b.Property<DateTime>("ModifiedDate");

                    b.Property<string>("RegistrationNumber");

                    b.HasKey("Id");

                    b.ToTable("Vehicles");
                });

            modelBuilder.Entity("TransportSystems.Backend.Core.Domain.Core.Billing.Basket", b =>
                {
                    b.OwnsOne("DotNetDistance.Distance", "Distance", b1 =>
                        {
                            b1.Property<int>("BasketId");

                            b1.Property<double>("Meters");

                            b1.HasKey("BasketId");

                            b1.ToTable("Baskets");

                            b1.HasOne("TransportSystems.Backend.Core.Domain.Core.Billing.Basket")
                                .WithOne("Distance")
                                .HasForeignKey("DotNetDistance.Distance", "BasketId")
                                .OnDelete(DeleteBehavior.Cascade);
                        });
                });

            modelBuilder.Entity("TransportSystems.Backend.Core.Domain.Core.Routing.RouteLeg", b =>
                {
                    b.OwnsOne("DotNetDistance.Distance", "Distance", b1 =>
                        {
                            b1.Property<int>("RouteLegId");

                            b1.Property<double>("Meters");

                            b1.HasKey("RouteLegId");

                            b1.ToTable("RouteLegs");

                            b1.HasOne("TransportSystems.Backend.Core.Domain.Core.Routing.RouteLeg")
                                .WithOne("Distance")
                                .HasForeignKey("DotNetDistance.Distance", "RouteLegId")
                                .OnDelete(DeleteBehavior.Cascade);
                        });
                });
#pragma warning restore 612, 618
        }
    }
}

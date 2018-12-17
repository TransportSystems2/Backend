﻿using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TransportSystems.Backend.Core.Domain.Core.Billing;
using TransportSystems.Backend.Core.Domain.Core.Catalogs;
using TransportSystems.Backend.Core.Domain.Core.Geo;
using TransportSystems.Backend.Core.Domain.Core.Ordering;
using TransportSystems.Backend.Core.Domain.Core.Organization;
using TransportSystems.Backend.Core.Domain.Core.Pricing;
using TransportSystems.Backend.Core.Domain.Core.Routing;
using TransportSystems.Backend.Core.Domain.Core.Trading;
using TransportSystems.Backend.Core.Domain.Core.Transport;
using TransportSystems.Backend.Core.Domain.Core.Users;
using TransportSystems.Backend.Core.Domain.Interfaces;

namespace TransportSystems.Backend.Core.Infrastructure.Database
{
    public class ApplicationContext : DbContext, IContext
    {
        public DbSet<City> Cities { get; set; }

        public DbSet<Company> Companies { get; set; }

        public DbSet<Garage> Garages { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<Route> Routes { get; set; }

        public DbSet<RouteLeg> RouteLegs { get; set; }

        public DbSet<Track> Paths { get; set; }

        public DbSet<TrackPoint> PathPoints { get; set; }

        public DbSet<Address> Address { get; set; }

        public DbSet<Cargo> Cargoes { get; set; }

        public DbSet<Moderator> Moderators { get; set; }

        public DbSet<Dispatcher> Dispatchers { get; set; }

        public DbSet<Driver> Drivers { get; set; }

        public DbSet<Customer> Customers { get; set; }

        public DbSet<Vehicle> Vehicles { get; set; }

        public DbSet<Pricelist> Pricelists { get; set; }

        public DbSet<Price> Prices { get; set; }

        public DbSet<OrderState> OrderStates { get; set; }

        public DbSet<LotRequest> LotRequests { get; set; }

        public DbSet<Lot> Lots { get; set; }

        public DbSet<Basket> Baskets { get; set; }

        public DbSet<Bill> Bills { get; set; }

        public DbSet<BillItem> BillItems { get; set; }

        public DbSet<Catalog> Catalogs { get; set; }

        public DbSet<CatalogItem> CatalogItems { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
        }

        public async Task<ITransaction> BeginTransaction()
        {
            var contextTransaction = await Database.BeginTransactionAsync();

            return new Transaction(contextTransaction);
        }
    }
}
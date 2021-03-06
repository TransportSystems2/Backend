﻿using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Users;
using TransportSystems.Backend.Application.Models.Users;

namespace TransportSystems.Backend.Application.Interfaces.Users
{
    public interface IApplicationUserService : IApplicationTransactionService
    {
        Task<Customer> GetOrCreateDomainCustomer(CustomerAM customer);

        Task<Moderator> GetDomainModerator(int moderatorId);

        Task<Dispatcher> GetDomainDispatcher(int dispatcherId);

        Task<CustomerAM> GetCustomer(int customerId);
        
        Task<Dispatcher> CreateDomainDispatcher(DispatcherAM dispatcher, int companyId);
        
        Task<Driver> CreateDomainDriver(DriverAM driver, int companyId);
    }
}
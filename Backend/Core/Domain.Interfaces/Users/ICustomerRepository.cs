﻿using TransportSystems.Backend.Core.Domain.Core.Users;

namespace TransportSystems.Backend.Core.Domain.Interfaces.Users
{
    public interface  ICustomerRepository : IIdentityUserRepository<Customer>
    {
    }
}
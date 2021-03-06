﻿using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Organization;

namespace TransportSystems.Backend.Core.Services.Interfaces.Organization
{
    public interface ICompanyService : IDomainService<Company>
    {
        Task<Company> Create(string name);

        Task<Company> GetByName(string name);
    }
}
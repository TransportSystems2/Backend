﻿using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Organization;

namespace TransportSystems.Backend.Core.Domain.Interfaces.Organization
{
    public interface ICompanyRepository : IRepository<Company>
    {
        Task<Company> GetByName(string name);
    }
}
﻿using Common.Models;
using Common.Models.Geolocation;
using System.Threading.Tasks;
using TransportSystems.Backend.External.Interfaces.Provider;

namespace TransportSystems.Backend.External.Interfaces.Maps
{
    public interface IMaps : IProvider
    {
        Task<TimeBelt> GetTimeBelt(Coordinate coordinate);
    }
}
﻿using TransportSystems.Backend.Application.Models.Geo;
using TransportSystems.Backend.Application.Models.Transport;
using TransportSystems.Backend.Application.Models.Users;

namespace TransportSystems.Backend.Application.Models.SignUp
{
    public class CompanyApplicationAM : BaseAM
    {
        public DispatcherAM Dispatcher { get; set; }

        public AddressAM GarageAddress { get; set; }

        public VehicleAM Vehicle { get; set; }

        public DriverAM Driver { get; set; }
    }
}
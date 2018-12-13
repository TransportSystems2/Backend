using System;
using System.Collections.Generic;
using System.Text;
using TransportSystems.Backend.Application.Models.Geo;

namespace TransportSystems.Backend.Application.Models.Organization
{
    public class GarageAM : DomainAM
    {
        public AddressAM Address { get; set; }
    }
}
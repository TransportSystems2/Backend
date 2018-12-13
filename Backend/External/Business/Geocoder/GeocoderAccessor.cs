using System;
using TransportSystems.Backend.External.Business.Provider;
using TransportSystems.Backend.External.Interfaces.Geocoder;
using TransportSystems.Backend.External.Models.Enums;

namespace TransportSystems.Backend.External.Business.Geocoder
{
    public class GeocoderAccessor : ProviderAccessor<IGeocoder>, IGeocoderAccessor
    {
        public GeocoderAccessor(Func<ProviderKind, IGeocoder> acceessor)
            : base(acceessor)
        {
        }
    }
}
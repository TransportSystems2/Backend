using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TransportSystems.Backend.External.Interfaces.Geocoder;
using TransportSystems.Backend.External.Interfaces.Services.Geocoder;
using TransportSystems.Backend.External.Models.Enums;
using TransportSystems.Backend.External.Models.Geo;

namespace TransportSystems.Backend.External.Business.Geocoder
{
    public class GeocoderService : IGeocoderService
    {
        public GeocoderService(IGeocoderAccessor providerAccessor)
        {
            ProviderAccessor = providerAccessor;
        }

        protected IGeocoderAccessor ProviderAccessor { get; }

        public ProviderKind[] DefaultProvidersKind { get => new[] { ProviderKind.Yandex, ProviderKind.Google, ProviderKind.DaData }; }

        public async Task<ICollection<AddressEM>> Geocode(string request, int maxResultCount = 5, params ProviderKind[] providersKind)
        {
            if (providersKind.Length == 0)
            {
                providersKind = DefaultProvidersKind;
            }

            foreach (var providerKind in providersKind)
            {
                var provider = ProviderAccessor.GetProvider(providerKind);
                if (provider == null)
                {
                    continue;
                }

                var result = await provider.Geocode(request, maxResultCount);
                if (result.Count > 0)
                {
                    return result;
                }
            }

            return new List<AddressEM>();
        }

        public async Task<ICollection<AddressEM>> ReverseGeocode(double latitude, double longitude, params ProviderKind[] providersKind)
        {
            if (providersKind.Length == 0)
            {
                providersKind = DefaultProvidersKind;
            }

            foreach (var providerKind in providersKind)
            {
                var provider = ProviderAccessor.GetProvider(providerKind);
                if (provider == null)
                {
                    continue;
                }

                var result = await provider.ReverseGeocode(latitude, longitude);
                if (result.Count > 0)
                {
                    return result;
                }
            }

            return new List<AddressEM>();
        }
    }
}
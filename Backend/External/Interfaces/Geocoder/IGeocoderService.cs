using System.Collections.Generic;
using System.Threading.Tasks;
using TransportSystems.Backend.External.Models.Enums;
using TransportSystems.Backend.External.Models.Geo;

namespace TransportSystems.Backend.External.Interfaces.Services.Geocoder
{
    public interface IGeocoderService
    {
        ProviderKind[] DefaultProvidersKind { get; }

        Task<ICollection<AddressEM>> Geocode(string request, int maxResultCount = 5, params ProviderKind[] providersKind);

        Task<ICollection<AddressEM>> ReverseGeocode(double latitude, double longitude, params ProviderKind[] providersKind);
    }
}

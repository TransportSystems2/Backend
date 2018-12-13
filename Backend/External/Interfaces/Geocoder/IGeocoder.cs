using System.Collections.Generic;
using System.Threading.Tasks;
using TransportSystems.Backend.External.Interfaces.Provider;
using TransportSystems.Backend.External.Models.Geo;

namespace TransportSystems.Backend.External.Interfaces.Geocoder
{
    public interface IGeocoder : IProvider
    {
        Task<ICollection<AddressEM>> Geocode(string request, int maxResultCount = 5);

        Task<ICollection<AddressEM>> ReverseGeocode(double latitude, double longitude);
    }
}
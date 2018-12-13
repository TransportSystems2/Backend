using GoogleApi;
using GoogleApi.Entities.Common.Enums;
using GoogleApi.Entities.Maps.Geocoding.Address.Request;
using GoogleApi.Entities.Maps.Geocoding.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TransportSystems.Backend.External.Interfaces.Geocoder;
using TransportSystems.Backend.External.Models.Enums;
using TransportSystems.Backend.External.Models.Geo;

namespace TransportSystems.Backend.External.Business.Geocoder.Providers
{
    public class GoogleGeocoder : IGeocoder
    {
        public ProviderKind Kind => ProviderKind.Google;

        public async Task<ICollection<AddressEM>> Geocode(string request, int maxResultCount = 5)
        {
            var result = new List<AddressEM>();

            var clientRequest = new AddressGeocodeRequest { Address = request, Language = Language.Russian, Key = GoogleConfig.ApiKey};
            var clientResponse = await GoogleMaps.AddressGeocode.QueryAsync(clientRequest);

            foreach (var geocodeResult in clientResponse.Results)
            {
                var address = CreateAddressByAddressComponents(geocodeResult);

                result.Add(address);
            }

            return result;
        }

        public Task<ICollection<AddressEM>> ReverseGeocode(double latitude, double longitude)
        {
            throw new NotImplementedException();
        }

        public AddressEM CreateAddressByAddressComponents(Result geocodeResult)
        {
            var addressComponents = geocodeResult.AddressComponents;

            var country = addressComponents.FirstOrDefault(c => c.Types.Contains(AddressComponentType.Country));
            var area1 = addressComponents.LastOrDefault(c => c.Types.Contains(AddressComponentType.Administrative_Area_Level_1));
            var area2 = addressComponents.FirstOrDefault(c => c.Types.Contains(AddressComponentType.Administrative_Area_Level_2));
            var locality = addressComponents.LastOrDefault(c => c.Types.Contains(AddressComponentType.Locality));
            var street = addressComponents.FirstOrDefault(c => c.Types.Contains(AddressComponentType.Route));
            var house = addressComponents.FirstOrDefault(c => c.Types.Contains(AddressComponentType.Street_Number));

            var result = new AddressEM
            {
                Country = country?.LongName,
                Province = area1?.LongName,
                Area = area2?.LongName,
                Locality = locality?.LongName,
                Street = street?.LongName,
                House = house?.LongName,
                Latitude = geocodeResult.Geometry.Location.Latitude,
                Longitude = geocodeResult.Geometry.Location.Longitude
            };

            return result;
        }
    }
}
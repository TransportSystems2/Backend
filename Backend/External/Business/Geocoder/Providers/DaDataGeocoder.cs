using DaData.Client;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using TransportSystems.Backend.External.Interfaces.Geocoder;
using TransportSystems.Backend.External.Models.Enums;
using TransportSystems.Backend.External.Models.Geo;

namespace TransportSystems.Backend.External.Business.Geocoder.Providers
{
    public class DaDataGeocoder : IGeocoder
    {
        public ProviderKind Kind => ProviderKind.DaData;

        public string ApiKey => "b6dd2fbec849b949ba1702261294853289a1e106";

        public string HostUrl => "https://suggestions.dadata.ru/suggestions/api/4_1/rs";

        public async Task<ICollection<AddressEM>> Geocode(string request, int maxResultCount = 5)
        {
            var result = new List<AddressEM>();

            var client = new SuggestClient(ApiKey, HostUrl);
            var clientRequst = new AddressSuggestQuery(request) { Count = maxResultCount };
            var response = await client.QueryAddress(clientRequst);

            foreach(var suggest in response.Suggestions)
            {
                var address = CreateAddressByAddressData(suggest.Data);

                result.Add(address);
            }

            return result;
        }

        public Task<ICollection<AddressEM>> ReverseGeocode(double latitude, double longitude)
        {
            throw new NotImplementedException();
        }

        public AddressEM CreateAddressByAddressData(AddressData addressData)
        {
            var result = new AddressEM
            {
                Country = addressData.Country,
                Province = addressData.RegionWithType,
                Area = addressData.AreaWithType,
                Locality = addressData.CityWithType,
                Street = addressData.StreetWithType,
                House = addressData.House
            };

            if (string.IsNullOrEmpty(result.Locality))
            {
                result.Locality = addressData.SettlementWithType;
            }

            if (!string.IsNullOrEmpty(addressData.GeoLat))
            {
                result.Latitude = Double.Parse(addressData.GeoLat, CultureInfo.InvariantCulture);
            }

            if (!string.IsNullOrEmpty(addressData.GeoLon))
            {
                result.Longitude = Double.Parse(addressData.GeoLon, CultureInfo.InvariantCulture);
            }

            return result;
        }
    }
}
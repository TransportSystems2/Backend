using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TransportSystems.Backend.External.Interfaces.Geocoder;
using TransportSystems.Backend.External.Models.Enums;
using TransportSystems.Backend.External.Models.Geo;
using Yandex.Geocoder;
using Yandex.Geocoder.Enums;
using Yandex.Geocoder.Models;

namespace TransportSystems.Backend.External.Business.Geocoder.Providers
{
    public class YandexGeocoder : IGeocoder
    {
        public ProviderKind Kind => ProviderKind.Yandex;

        public async Task<ICollection<AddressEM>> Geocode(string request, int maxResultCount = 5)
        {
            var result = new List<AddressEM>();

            var clientRequest = new GeocoderRequest { Request = request, MaxCount = maxResultCount };
            var client = new GeocoderClient();

            var clientResponse = await client.Geocode(clientRequest);
            
            foreach (var geoFeatureMember in clientResponse.GeoObjectCollection.FeatureMember)
            {
                var geoObject = geoFeatureMember.GeoObject;
                var address = CreateAddressByGeoObject(geoObject);

                result.Add(address);
            }

            return result;
        }

        public Task<ICollection<AddressEM>> ReverseGeocode(double latitude, double longitude)
        {
            throw new NotImplementedException();
        }

        public AddressEM CreateAddressByGeoObject(GeoObjectType geoObject)
        {
            var addressComponents = geoObject.MetaDataProperty.GeocoderMetaData.Address.Components;
            var country = addressComponents.FirstOrDefault(c => c.Kind.Equals(AddressComponentKind.Country));
            var province = addressComponents.LastOrDefault(c => c.Kind.Equals(AddressComponentKind.Province));
            var area = addressComponents.FirstOrDefault(c => c.Kind.Equals(AddressComponentKind.Area));
            var locality = addressComponents.LastOrDefault(c => c.Kind.Equals(AddressComponentKind.Locality));
            var street = addressComponents.FirstOrDefault(c => c.Kind.Equals(AddressComponentKind.Street));
            var house = addressComponents.FirstOrDefault(c => c.Kind.Equals(AddressComponentKind.House));
            var coordinate = new Coordinate(geoObject.Point.Pos);

            var result = new AddressEM
            {
                Country = country?.Name,
                Province = province?.Name,
                Area = area?.Name,
                Locality = locality?.Name,
                Street = street?.Name,
                House = house?.Name,
                Latitude = coordinate.Latitude,
                Longitude = coordinate.Longitude
            };

            return result;
        }
    }
}

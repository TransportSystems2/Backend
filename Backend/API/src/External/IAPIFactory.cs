using System.Collections.Generic;
using System.Net.Http;
using Refit;

namespace TransportSystems.Backend.API.External
{
    public static class IAPIFactory<T>
    {
        static readonly Dictionary<string, T> dict = new Dictionary<string, T>();

        public static T Create(HttpClient client)
        {
            var baseAddress = client.BaseAddress.AbsoluteUri;
            if (!dict.TryGetValue(baseAddress, out T api))
            {
                api = RestService.For<T>(client);
                dict.Add(baseAddress, api);
            }

            return api;
        }

        public static T Create(string baseAddress)
        {
            if (!dict.TryGetValue(baseAddress, out T api))
            {
                api = RestService.For<T>(baseAddress);
                dict.Add(baseAddress, api);
            }

            return api;
        }
    }
}

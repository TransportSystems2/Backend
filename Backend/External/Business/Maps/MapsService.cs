using Common.Models;
using Common.Models.Geolocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TransportSystems.Backend.External.Interfaces.Maps;
using TransportSystems.Backend.External.Interfaces.Services.Maps;
using TransportSystems.Backend.External.Models.Enums;

namespace TransportSystems.Backend.External.Business.Maps
{
    public class MapsService : IMapsService
    {
        public MapsService(IMapsAccessor mapsAccessor)
        {
            MapsAccessor = mapsAccessor;
        }

        public ProviderKind[] DefaultProvidersKind => new[] { ProviderKind.Google };

        protected IMapsAccessor MapsAccessor { get; }

        public async Task<TimeBelt> GetTimeBelt(Coordinate coordinate, params ProviderKind[] providersKind)
        {
            if (providersKind.Length == 0)
            {
                providersKind = DefaultProvidersKind;
            }

            var exceptions = new Queue<Exception>();

            foreach (var providerKind in providersKind)
            {
                try
                {
                    var provider = MapsAccessor.GetProvider(providerKind);
                    if (provider == null)
                    {
                        continue;
                    }

                    return await provider.GetTimeBelt(coordinate).ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    exceptions.Enqueue(e);
                }
            }

            if (exceptions.Any())
            {
                throw new AggregateException(exceptions);
            }
            else
            {
                throw new ArgumentException("No one providers does not exist", "ProvidersKind");
            }
        }
    }
}
using System;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityServer4.Configuration;
using IdentityServer4.Events;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using TransportSystems.Backend.Identity.Core.Interfaces;

namespace TransportSystems.Backend.Identity.Signin.Services
{
    public class HandlerService : DefaultEventService, IHandlerService
    {
        public HandlerService(
            IPushTokenService pushTokenService,
            IdentityServerOptions options,
            IHttpContextAccessor context,
            IEventSink sink,
            ISystemClock clock)
                : base(options, context, sink, clock)
        {
            PushTokenService = pushTokenService;
        }

        protected IPushTokenService PushTokenService { get; }

        public new async Task RaiseAsync(Event evt)
        {
            await base.RaiseAsync(evt);

            if (evt is UserLogoutSuccessEvent logoutEvent)
            {
                await PushTokenService.DeleteTokensByUser(int.Parse(logoutEvent.SubjectId));
            }
        }
    }
}

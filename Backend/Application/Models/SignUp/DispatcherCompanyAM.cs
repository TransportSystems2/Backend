using TransportSystems.Backend.Application.Models.Geo;
using TransportSystems.Backend.Application.Models.Users;

namespace TransportSystems.Backend.Application.Models.SignUp
{
    public class DispatcherCompanyAM : BaseAM
    {
        public AddressAM GarageAddress { get; set; }

        public string CompanyName { get; set; }

        public DispatcherAM Dispatcher { get; set; }
    }
}
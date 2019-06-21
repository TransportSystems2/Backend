using System.Threading.Tasks;
using TransportSystems.Backend.Core.Services.Interfaces;
using TransportSystems.Backend.Core.Services.Interfaces.Organization;
using TransportSystems.Backend.Application.Interfaces;
using TransportSystems.Backend.Application.Models.SignUp;
using TransportSystems.Backend.Application.Interfaces.Organization;
using TransportSystems.Backend.Application.Interfaces.Users;

namespace TransportSystems.Backend.Application.Business
{
    public class SignUpService : ApplicationTransactionService, ISignUpService
    {
        public SignUpService(
            ITransactionService transactionService,
            ICompanyService domainCompanyService,
            IApplicationUserService userService,
            IApplicationGarageService garageService,
            IApplicationVehicleService vehicleService)
            : base(transactionService)
        {
            DomainCompanyService = domainCompanyService;
            UserService = userService;
            GarageService = garageService;
            VehicleService = vehicleService;
        }

        protected ICompanyService DomainCompanyService { get; }

        protected IApplicationUserService UserService { get; }

        protected IApplicationGarageService GarageService { get; }

        protected IApplicationVehicleService VehicleService { get; }

        public async Task SignUpCompany(CompanyApplicationAM companyApplication)
        {
            using (var transaction = await TransactionService.BeginTransaction())
            {
                try
                {
                    var domainCompany = await DomainCompanyService.Create(
                        companyApplication.Dispatcher.PhoneNumber);
                 
                    var domainGarage = await GarageService.CreateDomainGarage(
                        domainCompany.Id,
                        companyApplication.GarageAddress
                    );

                    var domainVehicle = await VehicleService.CreateDomainVehicle(
                        domainCompany.Id,
                        companyApplication.Vehicle);

                    var domainDispatcher = await UserService.CreateDomainDispatcher(
                        companyApplication.Dispatcher,
                        domainCompany.Id
                    );

                    var domainDriver = await UserService.CreateDomainDriver(
                        companyApplication.Driver,
                        domainCompany.Id,
                        domainVehicle.Id
                    );

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
    }
}
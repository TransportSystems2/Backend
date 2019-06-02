using System;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Organization;
using TransportSystems.Backend.Core.Services.Interfaces;
using TransportSystems.Backend.Core.Services.Interfaces.Organization;
using TransportSystems.Backend.Core.Services.Interfaces.Users;
using TransportSystems.Backend.Application.Interfaces;
using TransportSystems.Backend.Application.Models.SignUp;
using TransportSystems.Backend.Application.Interfaces.Organization;

namespace TransportSystems.Backend.Application.Business
{
    public class SignUpService : ApplicationTransactionService, ISignUpService
    {
        public SignUpService(
            ITransactionService transactionService,
            IDriverService domainDriverService,
            IModeratorService domainModeratorService,
            IDispatcherService domainDispatcherService,
            ICompanyService domainCompanyService,
            IApplicationGarageService garageService,
            IApplicationVehicleService vehicleService)
            : base(transactionService)
        {
            DomainDriverService = domainDriverService;
            DomainModeratorService = domainModeratorService;
            DomainDispatcherService = domainDispatcherService;
            DomainCompanyService = domainCompanyService;
            GarageService = garageService;
            VehicleService = vehicleService;
        }

        protected IDriverService DomainDriverService { get; }

        protected IModeratorService DomainModeratorService { get; }

        protected IDispatcherService DomainDispatcherService { get; }

        protected ICompanyService DomainCompanyService { get; }

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

                    var domainDispatcher = await DomainDispatcherService.Create(
                        companyApplication.Dispatcher.FirstName,
                        companyApplication.Dispatcher.LastName,
                        companyApplication.Dispatcher.PhoneNumber,
                        domainCompany.Id
                    );

                    var domainDriver = await DomainDriverService.Create(
                        companyApplication.Driver.FirstName,
                        companyApplication.Driver.LastName,
                        companyApplication.Driver.PhoneNumber,
                        domainCompany.Id
                    );

                    await DomainDriverService.AssignVehicle(domainDriver.Id, domainVehicle.Id);
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
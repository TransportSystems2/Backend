using System;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Organization;
using TransportSystems.Backend.Core.Services.Interfaces;
using TransportSystems.Backend.Core.Services.Interfaces.Organization;
using TransportSystems.Backend.Core.Services.Interfaces.Users;
using TransportSystems.Backend.Application.Interfaces;
using TransportSystems.Backend.Application.Models.SignUp;

namespace TransportSystems.Backend.Application.Business
{
    public class SignUpService : ApplicationTransactionService, ISignUpService
    {
        public SignUpService(
            ITransactionService transactionService,
            IDriverService domainDriverService,
            IDispatcherService domainDispatcherService,
            ICompanyService domainCompanyService,
            IGarageService domainGarageService,
            IApplicationVehicleService vehicleService)
            : base(transactionService)
        {
            DomainDriverService = domainDriverService;
            DomainDispatcherService = domainDispatcherService;
            DomainCompanyService = domainCompanyService;
            DomainGarageService = domainGarageService;
            VehicleService = vehicleService;
        }

        protected IDriverService DomainDriverService { get; }

        protected IDispatcherService DomainDispatcherService { get; }

        protected ICompanyService DomainCompanyService { get; }

        protected IGarageService DomainGarageService { get; }

        protected IApplicationVehicleService VehicleService { get; }

        public async Task SignUpDispatcherCompany(DispatcherCompanyAM dispatcherCompanyModel)
        {
            using (var transaction = await TransactionService.BeginTransaction())
            {
                try
                {
                    var domainGarage = await DomainGarageService.GetByAddress(
                        dispatcherCompanyModel.GarageAddress.Country,
                        dispatcherCompanyModel.GarageAddress.Province,
                        dispatcherCompanyModel.GarageAddress.Locality,
                        dispatcherCompanyModel.GarageAddress.District
                    );

                    var domainCompany = await DomainCompanyService.Create(
                        domainGarage.Id,
                        dispatcherCompanyModel.CompanyName);

                    var domainDispatcher = await DomainDispatcherService.Create(
                        dispatcherCompanyModel.Dispatcher.FirstName,
                        dispatcherCompanyModel.Dispatcher.LastName,
                        dispatcherCompanyModel.Dispatcher.PhoneNumber,
                        domainCompany.Id
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

        public async Task SignUpDriverCompany(DriverCompanyAM driverCompanyModel)
        {
            using (var transaction = await TransactionService.BeginTransaction())
            {
                try
                {
                    var domainGarage = await DomainGarageService.GetByAddress(
                        driverCompanyModel.GarageAddress.Country,
                        driverCompanyModel.GarageAddress.Province,
                        driverCompanyModel.GarageAddress.Locality,
                        driverCompanyModel.GarageAddress.District
                    );

                    var domainCompany = await DomainCompanyService.Create(
                        domainGarage.Id,
                        driverCompanyModel.CompanyName);

                    var domainVehicle = await VehicleService.CreateDomainVehicle(
                        domainCompany.Id,
                        driverCompanyModel.Vehicle);

                    var domainDispatcher = await DomainDispatcherService.Create(
                        driverCompanyModel.Driver.FirstName,
                        driverCompanyModel.Driver.LastName,
                        driverCompanyModel.Driver.PhoneNumber,
                        domainCompany.Id
                    );

                    var domainDriver = await DomainDriverService.Create(
                        driverCompanyModel.Driver.FirstName,
                        driverCompanyModel.Driver.LastName,
                        driverCompanyModel.Driver.PhoneNumber,
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
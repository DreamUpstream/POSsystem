using FluentValidation;
using POSsystem.Contracts.Data;
using POSsystem.Contracts.DTO;
using POSsystem.Contracts.Enum;

namespace POSsystem.Core.Validators
{
    public class CreateOrUpdateServiceReservationDTOValidator : AbstractValidator<CreateOrUpdateServiceReservationDTO>
    {
        private readonly IUnitOfWork _repository;

        public CreateOrUpdateServiceReservationDTOValidator(IUnitOfWork repository)
        {
            _repository = repository;

            RuleFor(x => x.Time).NotNull().WithMessage("Time is required");
            RuleFor(x => x.ReservationStatus).NotNull().WithMessage("Reservation status is required");
            RuleFor(x => x.ServiceId).NotNull().WithMessage("Service is required");
            RuleFor(x => x.TaxId).NotNull().WithMessage("TaxId is required");
            RuleFor(x => x.OrderId).NotNull().WithMessage("Order is required");
            RuleFor(x => x.EmployeeId).NotNull().WithMessage("Employee is required");
            RuleFor(x => x.Time).GreaterThan(DateTime.Now).WithMessage("Reservation must be done for the future");
            RuleFor(x => (ReservationStatus)x.ReservationStatus).IsInEnum().WithMessage("Reservation status value is incorrect");
            RuleFor(x => _repository.Services.Get(x.ServiceId)).NotNull().WithMessage("Existing Service must be selected");
            RuleFor(x => _repository.Orders.Get(x.OrderId)).NotNull().WithMessage("Existing Order must be selected");
            RuleFor(x => _repository.Employees.Get(x.EmployeeId)).NotNull().WithMessage("Existing Employee must be selected");
        }
    }   
}
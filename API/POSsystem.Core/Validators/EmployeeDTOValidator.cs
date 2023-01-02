using POSsystem.Contracts.DTO;
using FluentValidation;
using POSsystem.Contracts.Data;
using POSsystem.Contracts.Enum;

namespace POSsystem.Core.Validators
{
    public class EmployeeDTOValidator : AbstractValidator<EmployeeDTO>
    {
        private readonly IUnitOfWork _repository;
        public EmployeeDTOValidator(IUnitOfWork repository)
        {
            _repository = repository;
            
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required");
            RuleFor(x => x.EmailAddress).NotEmpty().WithMessage("EmailAddress is required");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required");
            RuleFor(x => x.EmailAddress).EmailAddress().WithMessage("Not a valid EmailAddress");
            RuleFor(x => (EmployeeStatus)x.Status).IsInEnum().WithMessage("Employee Status is incorrect");
            RuleFor(x => x.CompanyId).NotNull().WithMessage("Assigned Company Id is required");
            RuleFor(x => _repository.Companies.Get(x.CompanyId)).NotNull().WithMessage("Company with the specified id not found");
        }
    }
}

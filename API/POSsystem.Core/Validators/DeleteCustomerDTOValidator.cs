using FluentValidation;
using POSsystem.Contracts.Data;
using POSsystem.Contracts.DTO;
using POSsystem.Contracts.Enum;

namespace POSsystem.Core.Validators;

public class DeleteCustomerDTOValidator : AbstractValidator<int>
{
    private readonly IUnitOfWork _repository;
    public DeleteCustomerDTOValidator(IUnitOfWork repository)
    {
        _repository = repository;
        RuleFor(x => _repository.Customers.Get(x)).NotNull().WithMessage("Existing Customer must be provided");
    }
}

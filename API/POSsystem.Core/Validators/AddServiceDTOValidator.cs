using POSsystem.Contracts.DTO;
using FluentValidation;
using POSsystem.Contracts.Data;
using POSsystem.Contracts.Enum;

namespace POSsystem.Core.Validators
{
    public class AddServiceDTOValidator : AbstractValidator<AddServiceDTO>
    {
        private readonly IUnitOfWork _repository;
        public AddServiceDTOValidator(IUnitOfWork repository)
        {
            _repository = repository;
            
            RuleFor(x => _repository.Services.Get(x.Id)).NotNull().WithMessage("Existing Service must be selected");
        }
    }
}

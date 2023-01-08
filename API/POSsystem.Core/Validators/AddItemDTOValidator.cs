using POSsystem.Contracts.DTO;
using FluentValidation;
using POSsystem.Contracts.Data;
using POSsystem.Contracts.Enum;

namespace POSsystem.Core.Validators
{
    public class AddItemDTOValidator : AbstractValidator<AddItemDTO>
    {
        private readonly IUnitOfWork _repository;
        public AddItemDTOValidator(IUnitOfWork repository)
        {
            _repository = repository;
            
            RuleFor(x => _repository.Items.Get(x.Id)).NotNull().WithMessage("Existing Item must be selected");
        }
    }
}

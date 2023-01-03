using FluentValidation;
using POSsystem.Contracts.Data;
using POSsystem.Contracts.DTO;

namespace POSsystem.Core.Validators
{
    public class DeleteBranchDTOValidator : AbstractValidator<int>
    {
        private readonly IUnitOfWork _repository;
        public DeleteBranchDTOValidator(IUnitOfWork repository)
        {
            _repository = repository;
            RuleFor(x => _repository.Branches.Get(x)).NotNull().WithMessage("Existing Branch must be provided");
        }
    }
}
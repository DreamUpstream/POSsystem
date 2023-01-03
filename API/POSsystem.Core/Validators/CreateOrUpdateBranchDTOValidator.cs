using FluentValidation;
using POSsystem.Contracts.Data;
using POSsystem.Contracts.DTO;
using POSsystem.Contracts.Enum;

namespace POSsystem.Core.Validators
{
    public class CreateOrUpdateBranchDTOValidator : AbstractValidator<CreateOrUpdateBranchDTO>
    {
        private readonly IUnitOfWork _repository;
        public CreateOrUpdateBranchDTOValidator(IUnitOfWork repository)
        {
            _repository = repository;
            RuleFor(x => x.Address).NotEmpty().WithMessage("Address is required");
            RuleFor(x=>x.Contacts).NotEmpty().WithMessage("Contacts is required"); 
            RuleFor(x=>(BranchStatus)x.Status).IsInEnum().WithMessage("Status is incorrect");

            RuleFor(x=>x.CompanyId).NotEmpty().WithMessage("Company id is required");
            RuleFor(x => _repository.Companies.Get(x.CompanyId)).NotNull()
                .WithMessage("Existing Company id must be provided");
            RuleFor(x => ValidateWorkingDays(x.BranchWorkingDays)).NotNull().WithMessage("Valid week days must be provided");
        }
        private bool? ValidateWorkingDays(IEnumerable<CreateOrUpdateBranchWorkingDayDTO> workingDays)
        {
            foreach (var day in workingDays)
            {
                if (!Enum.IsDefined(typeof(WorkingDay), day.WorkingDay))
                {
                    return null;
                }
            }
            return true;
        }
    }
}
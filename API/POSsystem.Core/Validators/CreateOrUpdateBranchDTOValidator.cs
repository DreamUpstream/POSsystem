using FluentValidation;
using POSsystem.Contracts.DTO;

namespace POSsystem.Core.Validators
{
    public class CreateOrUpdateBranchDTOValidator : AbstractValidator<CreateOrUpdateBranchDTO>
    {
        public CreateOrUpdateBranchDTOValidator()
        {
            RuleFor(x => x.Address).NotEmpty().WithMessage("Address is required");
            RuleFor(x=>x.Status).NotEmpty().WithMessage("Status is required");
            RuleFor(x=>x.Contacts).NotEmpty().WithMessage("Contacts is required"); 
            RuleFor(x=>x.BranchWorkingDays).NotEmpty().WithMessage("Branch working days are required");
        }
        private bool BeAValidDate(string value)
        {
            DateTime date;
            return DateTime.TryParse(value, out date);
        }
    }
}
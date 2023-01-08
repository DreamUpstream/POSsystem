using POSsystem.Contracts.DTO;
using FluentValidation;

namespace POSsystem.Core.Validators
{
    public class UserDTOValidator : AbstractValidator<ValidateUserDTO>
    {
        public UserDTOValidator()
        {
            RuleFor(x => x.EmailAddress).NotEmpty().WithMessage("EmailAddress is required");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required");
            RuleFor(x => x.EmailAddress).EmailAddress().WithMessage("Not a valid EmailAddress");
        }
    }
}

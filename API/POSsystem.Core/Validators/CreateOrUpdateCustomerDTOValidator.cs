using System.Text.RegularExpressions;
using FluentValidation;
using POSsystem.Contracts.Data;
using POSsystem.Contracts.DTO;
using POSsystem.Contracts.Enum;

namespace POSsystem.Core.Validators;

public class CreateOrUpdateCustomerDTOValidator : AbstractValidator<CreateOrUpdateCustomerDTO>
{
    private readonly IUnitOfWork _repository;
    public CreateOrUpdateCustomerDTOValidator(IUnitOfWork repository)
    {
        RuleFor(x => x.Name).NotNull().WithMessage("Name must be provided");
        RuleFor(x => x.PhoneNumber).NotNull().WithMessage("Phone number must be provided");
        RuleFor(x=> PhoneNumber.IsPhoneNbr(x.PhoneNumber)).NotNull().WithMessage("Phone number must consist of legal symbols only");
        // RuleFor(x => repository.Discounts.Get(x.DiscountId)).NotNull()
            // .WithMessage("Discount not found. Existing Discount must be provided");
        RuleFor(x => x.User.EmailAddress).EmailAddress().WithMessage("Correct Email address must be provided");
        RuleFor(x=>(UserRole)x.User.Role).IsInEnum().WithMessage("User role is incorrect");
        RuleFor(x=>(CustomerStatus)x.Status).IsInEnum().WithMessage("Customer Status is incorrect");
        RuleFor(x => DiscountIdExists(x.DiscountId, repository)).NotNull()
            .WithMessage("Specified discount Id must exist");
    }

    public bool? DiscountIdExists(int id, IUnitOfWork _repository)
    {
        if (_repository.Discounts.Get(id) != null)
        {
            return true;
        }
        else return null;
    }
}
public static class PhoneNumber
{
    // Regular expression used to validate a phone number.
    public const string motif = @"^[0-9]{7,10}$";

    public static bool? IsPhoneNbr(string number)
    {
        if (Regex.IsMatch(number, motif))
                return true;
            return null;
    }
}
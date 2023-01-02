using POSsystem.Contracts.DTO;
using FluentValidation;
using POSsystem.Contracts.Data;
using POSsystem.Contracts.Enum;

namespace POSsystem.Core.Validators
{
    public class DiscountDTOValidator : AbstractValidator<DiscountDTO>
    {
        private readonly IUnitOfWork _repository;
        public DiscountDTOValidator(IUnitOfWork repository)
        {
            _repository = repository;
            
            RuleFor(x => x.Amount).NotEmpty().WithMessage("Amount is required");
            RuleFor(x => x.Amount).NotEqual(0).WithMessage("Amount cannot be 0");
            RuleFor(x => (DiscountMeasure)x.Measure).IsInEnum().WithMessage("Discount Measure is incorrect");
            RuleFor(x => x.PromoCode).NotEmpty().WithMessage("A valid Promo Code is required");
            RuleFor(x => _repository.Discounts.GetAll().Where(d => d.PromoCode == x.PromoCode).Any())
                                                        .Null().WithMessage("A discount with this promo code already exists");
            RuleFor(x => x.EndTime).GreaterThan(x => x.StartTime).WithMessage("The end time cannot be before the start time.");


        }
    }
}

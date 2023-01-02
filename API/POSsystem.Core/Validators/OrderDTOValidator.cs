using POSsystem.Contracts.DTO;
using FluentValidation;
using POSsystem.Contracts.Data;
using POSsystem.Contracts.Enum;

namespace POSsystem.Core.Validators
{
    public class OrderDTOValidator : AbstractValidator<OrderDTO>
    {
        private readonly IUnitOfWork _repository;
        public OrderDTOValidator(IUnitOfWork repository)
        {
            _repository = repository;
            
            RuleFor(x => x.Tip).GreaterThanOrEqualTo(0).WithMessage("Tip cannot be a negative number");
            RuleFor(x => x.Tip).ScalePrecision(2, Int32.MaxValue, true).WithMessage("Tip cannot exceed 2 numbers after decimal point");
            RuleFor(x => x.DeliveryRequired).NotEmpty().WithMessage("Delivery condition must be specified");
            RuleFor(x => (OrderStatus)x.Status).NotEmpty().WithMessage("Order status must be specified");
            RuleFor(x => (OrderStatus)x.Status).IsInEnum().WithMessage("Order status is incorrect");
            RuleFor(x => x.CustomerId).NotEmpty().WithMessage("Customer Id must be specified");
            RuleFor(x => _repository.Customers.Get(x.CustomerId)).NotNull()
                .WithMessage("A customer with the id specified could not be found");
            RuleFor(x => x.EmployeeId).NotEmpty().WithMessage("Employee Id must be specified");
            RuleFor(x => _repository.Employees.Get(x.EmployeeId)).NotNull()
                .WithMessage("An employee with the id specified could not be found");
            RuleFor(x => x.DiscountId).NotEqual(x => _repository.Discounts.Get(x.DiscountId).Id)
                .WithMessage("Specified discount id is incorrect");
            RuleFor(x => x.Products).Must(x => x.All(id => _repository.Items.Get(id) != null))
                .WithMessage("Specified product id is incorrect");
            RuleFor(x => x.Services).Must(x => x.All(id => _repository.Services.Get(id) != null))
                .WithMessage("Specified service id is incorrect");
        }
    }
}

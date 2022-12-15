﻿using POSsystem.Contracts.DTO;
using FluentValidation;

namespace POSsystem.Core.Validators
{
    public class CreateOrUpdateItemDTOValidator : AbstractValidator<CreateOrUpdateItemDTO>
    {
        public CreateOrUpdateItemDTOValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required");
            RuleFor(x => x.Description).NotEmpty().WithMessage("Provide a brief description about the Item");
            RuleFor(x => x.CategoryId).NotEmpty().WithMessage("Provide category");
            RuleFor(x => x.ColorCode).NotEmpty().WithMessage("Tag a colorCode to the Item");
        }
    }
}

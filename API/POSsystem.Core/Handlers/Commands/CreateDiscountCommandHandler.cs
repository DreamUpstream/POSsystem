using MediatR;
using POSsystem.Contracts.Data;
using POSsystem.Contracts.DTO;
using POSsystem.Contracts.Enum;
using POSsystem.Contracts.Data.Entities;
using FluentValidation;
using POSsystem.Core.Exceptions;
using AutoMapper;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using POSsystem.Core.Services;

namespace POSsystem.Core.Handlers.Commands
{
    public class CreateDiscountCommand : IRequest<DiscountDTO>
    {
        public DiscountDTO Model { get; }
        public CreateDiscountCommand(DiscountDTO model)
        {
            Model = model;
        }
    }

    public class CreateDiscountCommandHandler : IRequestHandler<CreateDiscountCommand, DiscountDTO>
    {
        private readonly IUnitOfWork _repository;
        private readonly IValidator<DiscountDTO> _validator;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateDiscountCommandHandler> _logger;

        public CreateDiscountCommandHandler(ILogger<CreateDiscountCommandHandler> logger, IUnitOfWork repository, IValidator<DiscountDTO> validator, IMapper mapper)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<DiscountDTO> Handle(CreateDiscountCommand request, CancellationToken cancellationToken)
        {
            DiscountDTO model = request.Model;

            var result = _validator.Validate(model);

            _logger.LogInformation($"CreateDiscount Validation result: {result}");

            if (!result.IsValid)
            {
                var errors = result.Errors.Select(x => x.ErrorMessage).ToArray();
                throw new InvalidRequestBodyException
                {
                    Errors = errors
                };
            }

            var entity = new Discount
            {
                Amount = model.Amount,
                Measure = model.Measure,
                PromoCode = model.PromoCode,
                StartTime = model.StartTime,
                EndTime = model.EndTime
            };

            _repository.Discounts.Add(entity);
            await _repository.CommitAsync();

            return model;
        }
    }
}
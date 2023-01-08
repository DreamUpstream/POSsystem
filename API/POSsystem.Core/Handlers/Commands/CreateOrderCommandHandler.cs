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
    public class CreateOrderCommand : IRequest<CreateOrderDTO>
    {
        public CreateOrderDTO Model { get; }
        public CreateOrderCommand(CreateOrderDTO model)
        {
            Model = model;
        }
    }

    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, CreateOrderDTO>
    {
        private readonly IUnitOfWork _repository;
        private readonly IValidator<CreateOrderDTO> _validator;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateOrderCommandHandler> _logger;

        public CreateOrderCommandHandler(ILogger<CreateOrderCommandHandler> logger, IUnitOfWork repository, IValidator<CreateOrderDTO> validator, IMapper mapper)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<CreateOrderDTO> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            CreateOrderDTO model = request.Model;

            var result = _validator.Validate(model);

            _logger.LogInformation($"CreateOrder Validation result: {result}");

            if (!result.IsValid)
            {
                var errors = result.Errors.Select(x => x.ErrorMessage).ToArray();
                throw new InvalidRequestBodyException
                {
                    Errors = errors
                };
            }

            var items = _repository.Items.GetAll().Where(i => model.Products.Contains(i.Id));
            var services = _repository.Services.GetAll().Where(i => model.Services.Contains(i.Id));

            var entity = new Order
            {
                SubmissionDate = model.SubmissionDate,
                Tip = model.Tip,
                DeliveryRequired = model.DeliveryRequired,
                Comment = model.Comment,
                Status = model.Status,
                CustomerId = model.CustomerId,
                EmployeeId = model.EmployeeId,
                DiscountId = model.DiscountId,
                Delivery = model.Delivery
            };

            entity.Products.AddRange(items);
            entity.Services.AddRange(services);

            _repository.Orders.Add(entity);
            await _repository.CommitAsync();

            return model;
        }
    }
}
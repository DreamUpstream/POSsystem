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
    public class CreateOrderCommand : IRequest<OrderDTO>
    {
        public OrderDTO Model { get; }
        public CreateOrderCommand(OrderDTO model)
        {
            Model = model;
        }
    }

    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, OrderDTO>
    {
        private readonly IUnitOfWork _repository;
        private readonly IValidator<OrderDTO> _validator;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateOrderCommandHandler> _logger;

        public CreateOrderCommandHandler(ILogger<CreateOrderCommandHandler> logger, IUnitOfWork repository, IValidator<OrderDTO> validator, IMapper mapper)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<OrderDTO> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            OrderDTO model = request.Model;

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

            var items = _repository.Items.GetAll().Where(i => model.Products.Contains(i.Id)).ToList();
            var services = _repository.Services.GetAll().Where(i => model.Services.Contains(i.Id)).ToList();

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
                Delivery = model.Delivery,
                Products = items,
                Services = services
            };

            _repository.Orders.Add(entity);
            await _repository.CommitAsync();

            return model;
        }
    }
}
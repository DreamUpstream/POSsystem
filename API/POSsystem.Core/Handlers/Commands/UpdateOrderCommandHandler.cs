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
using POSsystem.Contracts.Services;

namespace POSsystem.Core.Handlers.Commands
{
    public class UpdateOrderCommand : IRequest<CreateOrderDTO>
    {
        public int Id { get; }
        public CreateOrderDTO Model { get; }
        public UpdateOrderCommand(int id, CreateOrderDTO model)
        {
            Id = id;
            Model = model;
        }
    }

    public class UpdateOrderCommandHandler : IRequestHandler<UpdateOrderCommand, CreateOrderDTO>
    {
        private readonly IUnitOfWork _repository;
        private readonly IValidator<CreateOrderDTO> _validator;
        private readonly IMapper _mapper;
        private readonly ICachingService _cache;
        private readonly ILogger<UpdateOrderCommandHandler> _logger;

        public UpdateOrderCommandHandler(ILogger<UpdateOrderCommandHandler> logger, IUnitOfWork repository, IValidator<CreateOrderDTO> validator, IMapper mapper, ICachingService cache)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
            _logger = logger;
            _cache = cache;
        }

        public async Task<CreateOrderDTO> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
        {
            CreateOrderDTO model = request.Model;
            var id = request.Id;

            var result = _validator.Validate(model);

            _logger.LogInformation($"UpdateOrder Validation result: {result}");

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

            var dbEntity = _repository.Orders.Get(id);

            if (dbEntity == null)
            {
                throw new EntityNotFoundException($"No Order found for the Id {id}");
            }

            dbEntity.Tip = model.Tip;
            dbEntity.DeliveryRequired = model.DeliveryRequired;
            dbEntity.Comment = model.Comment;
            dbEntity.Status = model.Status;
            dbEntity.CustomerId = model.CustomerId;
            dbEntity.EmployeeId = model.EmployeeId;
            dbEntity.DiscountId = dbEntity.DiscountId;
            dbEntity.Delivery = model.Delivery;
            dbEntity.Products = items;
            dbEntity.Services = services;

            _repository.Orders.Update(dbEntity);
            await _repository.CommitAsync();

            var updatedOrder = _mapper.Map<OrderDTO>(dbEntity);

            if (_cache.GetItem<OrderDTO>($"order_{id}") != null)
            {
                _logger.LogInformation($"Order Exists in Cache. Set new Item for the same Key.");
                _cache.SetItem($"order_{id}", updatedOrder);
            }

            return model;
        }
    }
}
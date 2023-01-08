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
    public class AddItemToOrderCommand : IRequest<AddItemDTO>
    {
        public int OrderId { get; }
        public AddItemDTO Model { get; }
        public AddItemToOrderCommand(int id, AddItemDTO model)
        {
            OrderId = id;
            Model = model;
        }
    }

    public class AddItemToOrderCommandHandler : IRequestHandler<AddItemToOrderCommand, AddItemDTO>
    {
        private readonly IUnitOfWork _repository;
        private readonly IValidator<AddItemDTO> _validator;
        private readonly IMapper _mapper;
        private readonly ICachingService _cache;
        private readonly ILogger<AddItemToOrderCommandHandler> _logger;

        public AddItemToOrderCommandHandler(ILogger<AddItemToOrderCommandHandler> logger, IUnitOfWork repository, IValidator<AddItemDTO> validator, IMapper mapper, ICachingService cache)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
            _logger = logger;
            _cache = cache;
        }

        public async Task<AddItemDTO> Handle(AddItemToOrderCommand request, CancellationToken cancellationToken)
        {
            AddItemDTO model = request.Model;
            var orderId = request.OrderId;

            var result = _validator.Validate(model);

            _logger.LogInformation($"AddItemToOrder Validation result: {result}");

            if (!result.IsValid)
            {
                var errors = result.Errors.Select(x => x.ErrorMessage).ToArray();
                throw new InvalidRequestBodyException
                {
                    Errors = errors
                };
            }

            var dbEntity = _repository.Orders.Get(orderId);

            if (dbEntity == null)
            {
                throw new EntityNotFoundException($"No Order found for the Id {orderId}");
            }

            var item = _repository.Items.Get(model.Id);

            dbEntity.Products.Add(item);

            _repository.Orders.Update(dbEntity);
            await _repository.CommitAsync();

            var updatedOrder = _mapper.Map<OrderDTO>(dbEntity);

            if (_cache.GetItem<OrderDTO>($"order_{orderId}") != null)
            {
                _logger.LogInformation($"Order Exists in Cache. Set new Item for the same Key.");
                _cache.SetItem($"order_{orderId}", updatedOrder);
            }

            return model;
        }
    }
}
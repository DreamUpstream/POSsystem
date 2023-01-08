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
    public class CancelOrderCommand : IRequest<CreateOrderDTO>
    {
        public int Id { get; }
        public CancelOrderCommand(int id)
        {
            Id = id;
        }
    }

    public class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand, CreateOrderDTO>
    {
        private readonly IUnitOfWork _repository;
        private readonly IValidator<CreateOrderDTO> _validator;
        private readonly IMapper _mapper;
        private readonly ICachingService _cache;
        private readonly ILogger<CancelOrderCommandHandler> _logger;

        public CancelOrderCommandHandler(ILogger<CancelOrderCommandHandler> logger, IUnitOfWork repository, IValidator<CreateOrderDTO> validator, IMapper mapper, ICachingService cache)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
            _logger = logger;
            _cache = cache;
        }

        public async Task<CreateOrderDTO> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
        {
            var id = request.Id;

            var dbEntity = _repository.Orders.Get(id);

            if (dbEntity == null)
            {
                throw new EntityNotFoundException($"No Order found for the Id {id}");
            }

            dbEntity.Status = OrderStatus.Canceled;

            _repository.Orders.Update(dbEntity);
            await _repository.CommitAsync();

            var updatedOrder = _mapper.Map<CreateOrderDTO>(dbEntity);

            if (_cache.GetItem<CreateOrderDTO>($"order_{id}") != null)
            {
                _logger.LogInformation($"Order Exists in Cache. Set new Item for the same Key.");
                _cache.SetItem($"order_{id}", updatedOrder);
            }

            return updatedOrder;
        }
    }
}
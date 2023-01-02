using AutoMapper;
using POSsystem.Contracts.Data;
using POSsystem.Contracts.DTO;
using MediatR;
using Microsoft.Extensions.Logging;
using POSsystem.Core.Exceptions;
using POSsystem.Contracts.Services;

namespace POSsystem.Core.Handlers.Queries
{
    public class GetOrderByIdQuery : IRequest<OrderDTO>
    {
        public int OrderId { get; }
        public GetOrderByIdQuery(int id)
        {
            OrderId = id;
        }
    }

    public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, OrderDTO>
    {
        private readonly IUnitOfWork _repository;
        private readonly IMapper _mapper;
        private readonly ICachingService _cache;
        private readonly ILogger<GetOrderByIdQueryHandler> _logger;

        public GetOrderByIdQueryHandler(ILogger<GetOrderByIdQueryHandler> logger, IUnitOfWork repository, IMapper mapper, ICachingService cache)
        {
            _repository = repository;
            _mapper = mapper;
            _cache = cache;
            _logger = logger;
        }

        public async Task<OrderDTO> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
        {
            var cachedOrder = _cache.GetItem<OrderDTO>($"order_{request.OrderId}");
            if (cachedOrder != null)
            {
                _logger.LogInformation($"Order {request.OrderId} Exists in Cache. Return CachedItem.");
                return cachedOrder;
            }

            _logger.LogInformation($"Order doesn't exist in Cache.");

            var order = await Task.FromResult(_repository.Orders.Get(request.OrderId));
            if (order == null)
            {
                throw new EntityNotFoundException($"No Order found with Id {request.OrderId}");
            }

            var result = _mapper.Map<OrderDTO>(order);

            _logger.LogInformation($"Add Order to Cache and return.");
            _cache.SetItem($"order_{request.OrderId}", result);
            return result;
        }
    }
}
using AutoMapper;
using POSsystem.Contracts.Data;
using POSsystem.Contracts.DTO;
using MediatR;
using Microsoft.Extensions.Logging;
using POSsystem.Core.Exceptions;
using POSsystem.Contracts.Services;

namespace POSsystem.Core.Handlers.Queries
{
    public class GetDiscountByIdQuery : IRequest<DiscountDTO>
    {
        public int DiscountId { get; }
        public GetDiscountByIdQuery(int id)
        {
            DiscountId = id;
        }
    }

    public class GetDiscountByIdQueryHandler : IRequestHandler<GetDiscountByIdQuery, DiscountDTO>
    {
        private readonly IUnitOfWork _repository;
        private readonly IMapper _mapper;
        private readonly ICachingService _cache;
        private readonly ILogger<GetDiscountByIdQueryHandler> _logger;

        public GetDiscountByIdQueryHandler(ILogger<GetDiscountByIdQueryHandler> logger, IUnitOfWork repository, IMapper mapper, ICachingService cache)
        {
            _repository = repository;
            _mapper = mapper;
            _cache = cache;
            _logger = logger;
        }

        public async Task<DiscountDTO> Handle(GetDiscountByIdQuery request, CancellationToken cancellationToken)
        {
            var cachedDiscount = _cache.GetItem<DiscountDTO>($"discount_{request.DiscountId}");
            if (cachedDiscount != null)
            {
                _logger.LogInformation($"Discount {request.DiscountId} Exists in Cache. Return CachedItem.");
                return cachedDiscount;
            }

            _logger.LogInformation($"Discount doesn't exist in Cache.");

            var discount = await Task.FromResult(_repository.Discounts.Get(request.DiscountId));
            if (discount == null)
            {
                throw new EntityNotFoundException($"No Discount found with Id {request.DiscountId}");
            }

            var result = _mapper.Map<DiscountDTO>(discount);

            _logger.LogInformation($"Add Discount to Cache and return.");
            _cache.SetItem($"discount_{request.DiscountId}", result);
            return result;
        }
    }
}
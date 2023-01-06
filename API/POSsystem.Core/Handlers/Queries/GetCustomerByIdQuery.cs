using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using POSsystem.Contracts.Data;
using POSsystem.Contracts.DTO;
using POSsystem.Contracts.Services;
using POSsystem.Core.Exceptions;

namespace POSsystem.Core.Handlers.Queries
{
    public class GetCustomerByIdQuery :IRequest<CustomerDTO>
    {
        public int CustomerId { get; }
        public GetCustomerByIdQuery(int id)
        {
            CustomerId = id;
        }
    }

    public class GetCustomerByIdQueryHandler : IRequestHandler<GetCustomerByIdQuery, CustomerDTO>
    {
        private readonly IUnitOfWork _repository;
        private readonly IMapper _mapper;
        private readonly ICachingService _cache;
        private readonly ILogger<GetCustomerByIdQueryHandler> _logger;

        public GetCustomerByIdQueryHandler(ILogger<GetCustomerByIdQueryHandler> logger, IUnitOfWork repository, IMapper mapper, ICachingService cache)
        {
            _repository = repository;
            _mapper = mapper;
            _cache = cache;
            _logger = logger;
        }

        public async Task<CustomerDTO> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
        {
            var cachedItem = _cache.GetItem<CustomerDTO>($"customer_{request.CustomerId}");
            if (cachedItem != null)
            {
                _logger.LogInformation($"Customer Exists in Cache. Return CachedItem.");
                return cachedItem;
            }

            _logger.LogInformation($"Customer doesn't exist in Cache.");

            var customer = await Task.FromResult(_repository.Customers.Get(request.CustomerId));
            if (customer == null)
            {
                throw new EntityNotFoundException($"No Customer found for Id {request.CustomerId}");
            }

            var result = _mapper.Map<CustomerDTO>(customer);

            _logger.LogInformation($"Add Branch to Cache and return.");
            var _ = _cache.SetItem($"customer_{request.CustomerId}", result);
            return result;
        }
    }
}

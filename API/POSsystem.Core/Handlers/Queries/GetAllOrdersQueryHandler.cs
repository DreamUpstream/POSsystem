using AutoMapper;
using POSsystem.Contracts.Data;
using POSsystem.Contracts.DTO;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace POSsystem.Core.Handlers.Queries
{
    public class GetAllOrdersQuery : IRequest<IEnumerable<OrderDTO>>
    {
    }

    public class GetAllOrdersQueryHandler : IRequestHandler<GetAllOrdersQuery, IEnumerable<OrderDTO>>
    {
        private readonly IUnitOfWork _repository;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _cache;

        public GetAllOrdersQueryHandler(IUnitOfWork repository, IMapper mapper, IDistributedCache cache)
        {
            _repository = repository;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<IEnumerable<OrderDTO>> Handle(GetAllOrdersQuery request, CancellationToken cancellationToken)
        {
            var cachedEntitiesString = await _cache.GetStringAsync("all_orders");
            
            if (cachedEntitiesString == null)
            {
                var entities = await Task.FromResult(_repository.Orders.GetAll());
                var result = _mapper.Map<IEnumerable<OrderDTO>>(entities);

                await _cache.SetStringAsync("all_orders", JsonConvert.SerializeObject(result));
                return result;
            }
            else
            {
                var cachedEntities = JsonConvert.DeserializeObject<IEnumerable<OrderDTO>>(cachedEntitiesString);
                return cachedEntities;
            }

        }
    }
}
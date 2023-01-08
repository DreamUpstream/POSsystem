using AutoMapper;
using POSsystem.Contracts.Data;
using POSsystem.Contracts.DTO;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace POSsystem.Core.Handlers.Queries
{
    public class GetAllDiscountsQuery : IRequest<IEnumerable<DiscountDTO>>
    {
    }

    public class GetAllDiscountsQueryHandler : IRequestHandler<GetAllDiscountsQuery, IEnumerable<DiscountDTO>>
    {
        private readonly IUnitOfWork _repository;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _cache;

        public GetAllDiscountsQueryHandler(IUnitOfWork repository, IMapper mapper, IDistributedCache cache)
        {
            _repository = repository;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<IEnumerable<DiscountDTO>> Handle(GetAllDiscountsQuery request, CancellationToken cancellationToken)
        {
            var cachedEntitiesString = await _cache.GetStringAsync("all_discounts");
            
            if (cachedEntitiesString == null)
            {
                var entities = await Task.FromResult(_repository.Discounts.GetAll());
                var result = _mapper.Map<IEnumerable<DiscountDTO>>(entities);

                await _cache.SetStringAsync("all_discounts", JsonConvert.SerializeObject(result));
                return result;
            }
            else
            {
                var cachedEntities = JsonConvert.DeserializeObject<IEnumerable<DiscountDTO>>(cachedEntitiesString);
                return cachedEntities;
            }

        }
    }
}
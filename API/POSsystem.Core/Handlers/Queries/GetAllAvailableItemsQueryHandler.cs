using AutoMapper;
using POSsystem.Contracts.Data;
using POSsystem.Contracts.DTO;
using POSsystem.Contracts.Enum;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace POSsystem.Core.Handlers.Queries
{
    public class GetAllAvailableItemsQuery : IRequest<IEnumerable<ItemDTO>>
    {
    }

    public class GetAllAvailableItemsQueryHandler : IRequestHandler<GetAllAvailableItemsQuery, IEnumerable<ItemDTO>>
    {
        private readonly IUnitOfWork _repository;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _cache;

        public GetAllAvailableItemsQueryHandler(IUnitOfWork repository, IMapper mapper, IDistributedCache cache)
        {
            _repository = repository;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<IEnumerable<ItemDTO>> Handle(GetAllAvailableItemsQuery request, CancellationToken cancellationToken)
        {
            var cachedEntitiesString = await _cache.GetStringAsync("all_available_items");
            
            if (cachedEntitiesString == null)
            {
                var entities = await Task.FromResult(_repository.Items.GetAll().Where(x => x.Status == ItemStatus.Available));
                var result = _mapper.Map<IEnumerable<ItemDTO>>(entities);

                await _cache.SetStringAsync("all_available_items", JsonConvert.SerializeObject(result));
                return result;
            }
            else
            {
                var cachedEntities = JsonConvert.DeserializeObject<IEnumerable<ItemDTO>>(cachedEntitiesString);
                return cachedEntities;
            }

        }
    }
}
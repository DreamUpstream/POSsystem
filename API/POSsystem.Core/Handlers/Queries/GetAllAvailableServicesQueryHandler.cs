using AutoMapper;
using POSsystem.Contracts.Data;
using POSsystem.Contracts.DTO;
using POSsystem.Contracts.Enum;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace POSsystem.Core.Handlers.Queries
{
    public class GetAllAvailableServicesQuery : IRequest<IEnumerable<ServiceDTO>>
    {
    }

    public class GetAllAvailableServicesQueryHandler : IRequestHandler<GetAllAvailableServicesQuery, IEnumerable<ServiceDTO>>
    {
        private readonly IUnitOfWork _repository;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _cache;

        public GetAllAvailableServicesQueryHandler(IUnitOfWork repository, IMapper mapper, IDistributedCache cache)
        {
            _repository = repository;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<IEnumerable<ServiceDTO>> Handle(GetAllAvailableServicesQuery request, CancellationToken cancellationToken)
        {
            var cachedEntitiesString = await _cache.GetStringAsync("all_available_services");
            
            if (cachedEntitiesString == null)
            {
                var entities = await Task.FromResult(_repository.Services.GetAll().Where(x => x.Status == ServiceStatus.Available));
                var result = _mapper.Map<IEnumerable<ServiceDTO>>(entities);

                await _cache.SetStringAsync("all_available_services", JsonConvert.SerializeObject(result));
                return result;
            }
            else
            {
                var cachedEntities = JsonConvert.DeserializeObject<IEnumerable<ServiceDTO>>(cachedEntitiesString);
                return cachedEntities;
            }

        }
    }
}
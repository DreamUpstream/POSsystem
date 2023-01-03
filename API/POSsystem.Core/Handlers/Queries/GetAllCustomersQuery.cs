using MediatR;
using POSsystem.Contracts.DTO;
using AutoMapper;
using POSsystem.Contracts.Data;
using POSsystem.Contracts.DTO;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace POSsystem.Core.Handlers.Queries
{}
    public class GetAllCustomersQuery : IRequest<IEnumerable<CustomerDTO>>
    {
        
    }
public class GetAllBranchesQueryHandler : IRequestHandler<GetAllCustomersQuery, IEnumerable<CustomerDTO>>
{
    private readonly IUnitOfWork _repository;
    private readonly IMapper _mapper;
    private readonly IDistributedCache _cache;
    
    public GetAllBranchesQueryHandler(IUnitOfWork repository, IMapper mapper, IDistributedCache cache)
    {
        _repository = repository;
        _mapper = mapper;
        _cache = cache;
    }
     
    public async Task<IEnumerable<CustomerDTO>> Handle(GetAllCustomersQuery request, CancellationToken cancellationToken)
    {
         
        var cachedEntitiesString = await _cache.GetStringAsync("all_customers");
             
        if (cachedEntitiesString == null)
        {
            var entities = await Task.FromResult(_repository.Customers.GetAll());
            var result = _mapper.Map<IEnumerable<CustomerDTO>>(entities);
         
            await _cache.SetStringAsync("all_customers", JsonConvert.SerializeObject(result));
            return result;
        }
        else
        {
            var cachedEntities = JsonConvert.DeserializeObject<IEnumerable<CustomerDTO>>(cachedEntitiesString);
            return cachedEntities;
        }

    }
}
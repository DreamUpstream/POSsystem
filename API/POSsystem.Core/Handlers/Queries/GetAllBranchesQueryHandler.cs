using System.IO.Enumeration;
using AutoMapper;
using POSsystem.Contracts.Data;
using POSsystem.Contracts.DTO;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace POSsystem.Core.Handlers.Queries;
public class GetAllBranchesQuery : IRequest<IEnumerable<BranchDTO>>
{
}

public class GetAllBranchesQueryHandler : IRequestHandler<GetAllBranchesQuery, IEnumerable<BranchDTO>>
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
     
     public async Task<IEnumerable<BranchDTO>> Handle(GetAllBranchesQuery request, CancellationToken cancellationToken)
     {
         
         var cachedEntitiesString = await _cache.GetStringAsync("all_branches");
             
         if (cachedEntitiesString == null)
         {
             var entities = await Task.FromResult(_repository.Branches.GetAll());
             var result = _mapper.Map<IEnumerable<BranchDTO>>(entities);
         
             await _cache.SetStringAsync("all_branches", JsonConvert.SerializeObject(result));
             return result;
         }
         else
         {
             var cachedEntities = JsonConvert.DeserializeObject<IEnumerable<BranchDTO>>(cachedEntitiesString);
             return cachedEntities;
         }

     }
}
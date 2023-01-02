using AutoMapper;
using POSsystem.Contracts.Data;
using POSsystem.Contracts.DTO;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System.Linq;

namespace POSsystem.Core.Handlers.Queries
{
    public class GetAllEmployeesQuery : IRequest<IEnumerable<EmployeeDTO>>
    {
    }

    public class GetAllEmployeesQueryHandler : IRequestHandler<GetAllEmployeesQuery, IEnumerable<EmployeeDTO>>
    {
        private readonly IUnitOfWork _repository;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _cache;

        public GetAllEmployeesQueryHandler(IUnitOfWork repository, IMapper mapper, IDistributedCache cache)
        {
            _repository = repository;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<IEnumerable<EmployeeDTO>> Handle(GetAllEmployeesQuery request, CancellationToken cancellationToken)
        {
            var cachedEntitiesString = await _cache.GetStringAsync("all_employees");
            
            if (cachedEntitiesString == null)
            {
                var entities = await Task.FromResult(_repository.Employees.GetAll());
                var result = _mapper.Map<IEnumerable<EmployeeDTO>>(entities);

                await _cache.SetStringAsync("all_employees", JsonConvert.SerializeObject(result));
                return result;
            }
            else
            {
                var cachedEntities = JsonConvert.DeserializeObject<IEnumerable<EmployeeDTO>>(cachedEntitiesString);
                return cachedEntities;
            }

        }
    }
}
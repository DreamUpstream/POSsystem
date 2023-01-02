using AutoMapper;
using POSsystem.Contracts.Data;
using POSsystem.Contracts.DTO;
using POSsystem.Contracts.Enum;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using POSsystem.Core.Exceptions;

namespace POSsystem.Core.Handlers.Queries
{
    public class GetAllActiveEmployeesQuery : IRequest<IEnumerable<EmployeeDTO>>
    {
        public int CompanyId { get; }

        public GetAllActiveEmployeesQuery(string id)
        {
            if(!int.TryParse(id, out int companyId))
                throw new ArgumentException("Company Id is incorrect.");
            CompanyId = companyId;
            
        }
    }

    public class GetAllActiveEmployeesQueryHandler : IRequestHandler<GetAllActiveEmployeesQuery, IEnumerable<EmployeeDTO>>
    {
        private readonly IUnitOfWork _repository;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _cache;

        public GetAllActiveEmployeesQueryHandler(IUnitOfWork repository, IMapper mapper, IDistributedCache cache)
        {
            _repository = repository;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<IEnumerable<EmployeeDTO>> Handle(GetAllActiveEmployeesQuery request, CancellationToken cancellationToken)
        {
            var cachedEntitiesString = await _cache.GetStringAsync($"all_active_employees_company_{request.CompanyId}");
            
            if (cachedEntitiesString == null)
            {
                var entities = await Task.FromResult(_repository.Employees.GetAll().Where(x => x.CompanyId == request.CompanyId 
                                                                                            && x.Status == EmployeeStatus.Active));

                if(entities == null)
                {
                    throw new EntityNotFoundException($"No Active Employees found with Company Id {request.CompanyId}");
                }

                var result = _mapper.Map<IEnumerable<EmployeeDTO>>(entities);

                await _cache.SetStringAsync($"all_active_employees_company_{request.CompanyId}", JsonConvert.SerializeObject(result));
                return result;
            }

            var cachedEntities = JsonConvert.DeserializeObject<IEnumerable<EmployeeDTO>>(cachedEntitiesString);
            return cachedEntities;

        }
    }
}
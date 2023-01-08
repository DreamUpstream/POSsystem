using AutoMapper;
using POSsystem.Contracts.Data;
using POSsystem.Contracts.DTO;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace POSsystem.Core.Handlers.Queries
{
    public class GetAllEmployeesByCompanyIdQuery : IRequest<IEnumerable<EmployeeDTO>>
    {
        public int CompanyId { get; }
        public GetAllEmployeesByCompanyIdQuery(int id)
        {
            CompanyId = id;
        }
    }

    public class GetAllEmployeesByCompanyIdQueryHandler : IRequestHandler<GetAllEmployeesByCompanyIdQuery, IEnumerable<EmployeeDTO>>
    {
        private readonly IUnitOfWork _repository;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _cache;
        private readonly ILogger<GetAllEmployeesByCompanyIdQueryHandler> _logger;

        public GetAllEmployeesByCompanyIdQueryHandler(ILogger<GetAllEmployeesByCompanyIdQueryHandler> logger, IUnitOfWork repository, IMapper mapper, IDistributedCache cache)
        {
            _repository = repository;
            _mapper = mapper;
            _cache = cache;
            _logger = logger;
        }

        public async Task<IEnumerable<EmployeeDTO>> Handle(GetAllEmployeesByCompanyIdQuery request, CancellationToken cancellationToken)
        {
            var cachedEmployeesString = await _cache.GetStringAsync($"employees_company_{request.CompanyId}");
            if (cachedEmployeesString != null)
            {
                _logger.LogInformation($"Employees in Company {request.CompanyId} Exists in Cache. Return Cached Employees.");
                return JsonConvert.DeserializeObject<IEnumerable<EmployeeDTO>>(cachedEmployeesString);
            }
            
            var entities = await Task.FromResult(_repository.Employees.GetAll().Where(x => x.CompanyId == request.CompanyId));
            var result = _mapper.Map<IEnumerable<EmployeeDTO>>(entities);

            await _cache.SetStringAsync($"employees_company_{request.CompanyId}", JsonConvert.SerializeObject(result));
            return result;
        }
    }
}
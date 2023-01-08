using AutoMapper;
using POSsystem.Contracts.Data;
using POSsystem.Contracts.DTO;
using MediatR;
using Microsoft.Extensions.Logging;
using POSsystem.Core.Exceptions;
using POSsystem.Contracts.Services;

namespace POSsystem.Core.Handlers.Queries
{
    public class GetEmployeeByIdQuery : IRequest<EmployeeDTO>
    {
        public int EmployeeId { get; }
        public GetEmployeeByIdQuery(int id)
        {
            EmployeeId = id;
        }
    }

    public class GetEmployeeByIdQueryHandler : IRequestHandler<GetEmployeeByIdQuery, EmployeeDTO>
    {
        private readonly IUnitOfWork _repository;
        private readonly IMapper _mapper;
        private readonly ICachingService _cache;
        private readonly ILogger<GetEmployeeByIdQueryHandler> _logger;

        public GetEmployeeByIdQueryHandler(ILogger<GetEmployeeByIdQueryHandler> logger, IUnitOfWork repository, IMapper mapper, ICachingService cache)
        {
            _repository = repository;
            _mapper = mapper;
            _cache = cache;
            _logger = logger;
        }

        public async Task<EmployeeDTO> Handle(GetEmployeeByIdQuery request, CancellationToken cancellationToken)
        {
            var cachedEmployee = _cache.GetItem<EmployeeDTO>($"employee_{request.EmployeeId}");
            if (cachedEmployee != null)
            {
                _logger.LogInformation($"Employee {request.EmployeeId} Exists in Cache. Return CachedItem.");
                return cachedEmployee;
            }

            _logger.LogInformation($"Employee doesn't exist in Cache.");

            var employees = await Task.FromResult(_repository.Employees.GetAll().Where(x=> x.UserId == request.EmployeeId));
            if (!employees.Any())
            {
                throw new EntityNotFoundException($"No Employee found with Id {request.EmployeeId}");
            }

            var employee = employees.First();
            var result = _mapper.Map<EmployeeDTO>(employee);

            _logger.LogInformation($"Add Employee to Cache and return.");
            _cache.SetItem($"employee_{request.EmployeeId}", result);
            return result;
        }
    }
}
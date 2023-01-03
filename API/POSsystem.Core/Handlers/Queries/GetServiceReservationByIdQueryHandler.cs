using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using POSsystem.Contracts.Data;
using POSsystem.Contracts.DTO;
using POSsystem.Contracts.Services;
using POSsystem.Core.Exceptions;

namespace POSsystem.Core.Handlers.Queries;

public class GetServiceReservationByIdQuery : IRequest<ServiceReservationDTO>
{
    public int ServiceReservationId { get; }

    public GetServiceReservationByIdQuery(int id)
    {
        ServiceReservationId = id;
    }
}
    
public class GetServiceReservationByIdQueryHandler : IRequestHandler<GetServiceReservationByIdQuery, ServiceReservationDTO>
{
    private readonly IUnitOfWork _repository;
    private readonly IMapper _mapper;
    private readonly ICachingService _cache;
    private readonly ILogger<GetServiceReservationByIdQueryHandler> _logger;

    public GetServiceReservationByIdQueryHandler(ILogger<GetServiceReservationByIdQueryHandler> logger, IUnitOfWork repository, IMapper mapper, ICachingService cache)
    {
        _repository = repository;
        _mapper = mapper;
        _cache = cache;
        _logger = logger;
    }

    public async Task<ServiceReservationDTO> Handle(GetServiceReservationByIdQuery request, CancellationToken cancellationToken)
    {
        var cachedItem = _cache.GetItem<ServiceReservationDTO>($"service_reservation_{request.ServiceReservationId}");
        if (cachedItem != null)
        {
            _logger.LogInformation($"ServiceReservation Exists in Cache. Return CachedServiceReservation.");
            return cachedItem;
        }
        
        _logger.LogInformation($"ServiceReservation doesn't exist in Cache.");

        var item = await Task.FromResult(_repository.ServiceReservations.Get(request.ServiceReservationId));
        if (item == null)
        {
            throw new EntityNotFoundException($"No ServiceResrvation found for Id {request.ServiceReservationId}");
        }

        var result = _mapper.Map<ServiceReservationDTO>(item);
        
        _logger.LogInformation($"Add ServiceReservation to Cache and return.");
        var _ = _cache.SetItem($"service_reservation_{request.ServiceReservationId}", result);
        return result;
    }
}
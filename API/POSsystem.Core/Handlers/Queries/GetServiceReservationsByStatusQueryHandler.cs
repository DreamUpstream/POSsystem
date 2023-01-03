using AutoMapper;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using POSsystem.Contracts.Data;
using POSsystem.Contracts.DTO;

namespace POSsystem.Core.Handlers.Queries;

public class GetServiceReservationsByStatusQuery : IRequest<IEnumerable<ServiceReservationDTO>>
{
    public int ServiceReservationStatus { get; }

    public GetServiceReservationsByStatusQuery(int status)
    {
        ServiceReservationStatus = status;
    }
}

public class GetServiceReservationsByStatusQueryHandler : IRequestHandler<GetServiceReservationsByStatusQuery, IEnumerable<ServiceReservationDTO>>
{
    private readonly IUnitOfWork _repository;
    private readonly IMapper _mapper;
    private readonly IDistributedCache _cache;
    private readonly ILogger<GetServiceReservationsByStatusQueryHandler> _logger;

    public GetServiceReservationsByStatusQueryHandler(ILogger<GetServiceReservationsByStatusQueryHandler> logger, IUnitOfWork repository, IMapper mapper, IDistributedCache cache)
    {
        _repository = repository;
        _mapper = mapper;
        _cache = cache;
        _logger = logger;
    }

    public async Task<IEnumerable<ServiceReservationDTO>> Handle(GetServiceReservationsByStatusQuery request, CancellationToken cancellationToken)
    {
        var cachedEntitiesString = await _cache.GetStringAsync($"service_reservations_with_status_{request.ServiceReservationStatus}");

        if (cachedEntitiesString == null)
        {
            _logger.LogInformation($"ServiceReservation list by status {request.ServiceReservationStatus} doesn't exist in Cache.");
            var entities = await Task.FromResult(_repository.ServiceReservations.GetAll().Where(reservation => (int)reservation.ReservationStatus == request.ServiceReservationStatus));
            var result = _mapper.Map<IEnumerable<ServiceReservationDTO>>(entities);

            _logger.LogInformation($"Add ServiceReservations with status {request.ServiceReservationStatus} to Cache and return.");
            await _cache.SetStringAsync($"service_reservations_with_status_{request.ServiceReservationStatus}", JsonConvert.SerializeObject(result));
            return result;
        }

        _logger.LogInformation($"ServiceReservation list by status {request.ServiceReservationStatus} exists in Cache.");
        var cachedEntities = JsonConvert.DeserializeObject<IEnumerable<ServiceReservationDTO>>(cachedEntitiesString);
        return cachedEntities;
    }

}
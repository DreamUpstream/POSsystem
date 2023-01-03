using AutoMapper;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using POSsystem.Contracts.Data;
using POSsystem.Contracts.DTO;

namespace POSsystem.Core.Handlers.Queries;

public class GetAllServiceReservationsQuery : IRequest<IEnumerable<ServiceReservationDTO>>
{
}

public class GetAllServiceReservationsQueryHandler : IRequestHandler<GetAllServiceReservationsQuery, IEnumerable<ServiceReservationDTO>>
{
    private readonly IUnitOfWork _repository;
    private readonly IMapper _mapper;
    private readonly IDistributedCache _cache;

    public GetAllServiceReservationsQueryHandler(IUnitOfWork repository, IMapper mapper, IDistributedCache cache)
    {
        _repository = repository;
        _mapper = mapper;
        _cache = cache;
    }

    public async Task<IEnumerable<ServiceReservationDTO>> Handle(GetAllServiceReservationsQuery request, CancellationToken cancellationToken)
    {
        var cachedEntitiesString = await _cache.GetStringAsync("all_service_reservations");

        if (cachedEntitiesString == null)
        {
            var entities = await Task.FromResult(_repository.ServiceReservations.GetAll());
            var result = _mapper.Map<IEnumerable<ServiceReservationDTO>>(entities);

            await _cache.SetStringAsync("all_service_reservations", JsonConvert.SerializeObject(result));
            return result;
        }

        var cachedEntities = JsonConvert.DeserializeObject<IEnumerable<ServiceReservationDTO>>(cachedEntitiesString);
        return cachedEntities;
    }
}
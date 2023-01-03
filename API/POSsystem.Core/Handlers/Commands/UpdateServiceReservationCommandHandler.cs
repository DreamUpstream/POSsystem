using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using POSsystem.Contracts.Data;
using POSsystem.Contracts.DTO;
using POSsystem.Contracts.Enum;
using POSsystem.Contracts.Services;
using POSsystem.Core.Exceptions;

namespace POSsystem.Core.Handlers.Commands;

public class UpdateServiceReservationCommand : IRequest<ServiceReservationDTO>
{
    public int ServiceReservationId { get; }
    public CreateOrUpdateServiceReservationDTO Model { get; }

    public UpdateServiceReservationCommand(int serviceReservationId, CreateOrUpdateServiceReservationDTO model)
    {
        ServiceReservationId = serviceReservationId;
        Model = model;
    }
}

public class UpdateServiceReservationCommandHandler : IRequestHandler<UpdateServiceReservationCommand, ServiceReservationDTO>
{
    private readonly IUnitOfWork _repository;
    private readonly IValidator<CreateOrUpdateServiceReservationDTO> _validator;
    private readonly IMapper _mapper;
    private readonly ICachingService _cache;
    private readonly ILogger<UpdateServiceReservationCommandHandler> _logger;

    public UpdateServiceReservationCommandHandler(ILogger<UpdateServiceReservationCommandHandler> logger, IUnitOfWork repository, IValidator<CreateOrUpdateServiceReservationDTO> validator, IMapper mapper, ICachingService cache)
    {
        _repository = repository;
        _validator = validator;
        _mapper = mapper;
        _cache = cache;
        _logger = logger;
    }

    async Task<ServiceReservationDTO> IRequestHandler<UpdateServiceReservationCommand, ServiceReservationDTO>.Handle(UpdateServiceReservationCommand request, CancellationToken cancellationToken)
    {
        CreateOrUpdateServiceReservationDTO model = request.Model;
        int serviceReservationId = request.ServiceReservationId;

        var result = _validator.Validate(model);
        
        _logger.LogInformation($"UpdateServiceReservation Validation result: {result}");

        if (!result.IsValid)
        {
            var errors = result.Errors.Select(x => x.ErrorMessage).ToArray();
            throw new InvalidRequestBodyException
            {
                Errors = errors
            };
        }

        var dbEntity = _repository.ServiceReservations.Get(serviceReservationId);

        if (dbEntity == null)
        {
            throw new EntityNotFoundException($"No ServiceReservation found for the Id {serviceReservationId}");
        }

        dbEntity.Time = model.Time.ToString("yyyy-MM-dd hh:mm:ss");
        dbEntity.ReservationStatus = model.ReservationStatus;
        dbEntity.ServiceId = model.ServiceId;
        dbEntity.TaxId = model.TaxId;
        dbEntity.OrderId = model.OrderId;
        dbEntity.EmployeeId = model.EmployeeId;
        
        _repository.ServiceReservations.Update(dbEntity);
        await _repository.CommitAsync();

        var updatedServiceReservation = _mapper.Map<ServiceReservationDTO>(dbEntity);
        
        // if a version exists in the Cache
        // replace Cached ServiceReservation with the Updated ServiceReservation
        if (_cache.GetItem<ServiceReservationDTO>($"service_reservation_{serviceReservationId}") != null)
        {
            _logger.LogInformation($"ServiceReservation exists in Cache. Set new ServiceReservation for the same Key.");
            _cache.SetItem($"service_reservation_{serviceReservationId}", updatedServiceReservation);
        }

        return updatedServiceReservation;
    }
}
using System.Globalization;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using POSsystem.Contracts.Data;
using POSsystem.Contracts.DTO;
using AutoMapper;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using POSsystem.Contracts.Data.Entities;
using POSsystem.Contracts.Enum;
using POSsystem.Core.Exceptions;

namespace POSsystem.Core.Handlers.Commands;

public class CreateServiceReservationCommand : IRequest<ServiceReservationDTO>
{
    public CreateOrUpdateServiceReservationDTO Dto { get; }

    public CreateServiceReservationCommand(CreateOrUpdateServiceReservationDTO dto)
    {
        Dto = dto;
    }
}
public class CreateServiceReservationCommandHandler : IRequestHandler<CreateServiceReservationCommand, ServiceReservationDTO>
{
    private readonly IUnitOfWork _repository;
    private readonly IValidator<CreateOrUpdateServiceReservationDTO> _validator;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateServiceReservationCommandHandler> _logger;
    private readonly IDistributedCache _cache;

    public CreateServiceReservationCommandHandler(ILogger<CreateServiceReservationCommandHandler> logger, IUnitOfWork repository, IValidator<CreateOrUpdateServiceReservationDTO> validator, IMapper mapper, IDistributedCache cache)
    {
        _repository = repository;
        _validator = validator;
        _mapper = mapper;
        _logger = logger;
        _cache = cache;
    }

    public async Task<ServiceReservationDTO> Handle(CreateServiceReservationCommand request, CancellationToken cancellationToken)
    {
        CreateOrUpdateServiceReservationDTO Dto = request.Dto;

        var result = _validator.Validate(Dto);
        
        _logger.LogInformation($"CreateServiceReservation Validation result: {result}");

        if (!result.IsValid)
        {
            var errors = result.Errors.Select(x => x.ErrorMessage).ToArray();
            throw new InvalidRequestBodyException
            {
                Errors = errors
            };
        }

        var entity = new ServiceReservation
        {
            Time = Dto.Time.ToString("yyyy-MM-dd hh:mm:ss"),
            ReservationStatus = Dto.ReservationStatus,
            ServiceId = Dto.ServiceId,
            TaxId = Dto.TaxId,
            OrderId = Dto.OrderId,
            EmployeeId = Dto.EmployeeId
        };

        _repository.ServiceReservations.Add(entity);
        await _repository.CommitAsync();

        var entities = await Task.FromResult(_repository.ServiceReservations.GetAll());
        var updatedResult = _mapper.Map<IEnumerable<ServiceReservationDTO>>(entities);

        await _cache.SetStringAsync("all_service_reservations", JsonConvert.SerializeObject(updatedResult));
        
        var smth = _mapper.Map<ServiceReservationDTO>(entity);
        Console.WriteLine(smth.ReservationStatus);
        return smth;
    }
}
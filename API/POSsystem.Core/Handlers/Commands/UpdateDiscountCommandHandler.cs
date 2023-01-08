using MediatR;
using POSsystem.Contracts.Data;
using POSsystem.Contracts.DTO;
using POSsystem.Contracts.Enum;
using POSsystem.Contracts.Data.Entities;
using FluentValidation;
using POSsystem.Core.Exceptions;
using AutoMapper;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using POSsystem.Core.Services;
using POSsystem.Contracts.Services;

namespace POSsystem.Core.Handlers.Commands
{
    public class UpdateDiscountCommand : IRequest<DiscountDTO>
    {
        public int Id { get; }
        public DiscountDTO Model { get; }
        public UpdateDiscountCommand(int id, DiscountDTO model)
        {
            Id = id;
            Model = model;
        }
    }

    public class UpdateDiscountCommandHandler : IRequestHandler<UpdateDiscountCommand, DiscountDTO>
    {
        private readonly IUnitOfWork _repository;
        private readonly IValidator<DiscountDTO> _validator;
        private readonly IMapper _mapper;
        private readonly ICachingService _cache;
        private readonly ILogger<UpdateDiscountCommandHandler> _logger;

        public UpdateDiscountCommandHandler(ILogger<UpdateDiscountCommandHandler> logger, IUnitOfWork repository, IValidator<DiscountDTO> validator, IMapper mapper, ICachingService cache)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
            _logger = logger;
            _cache = cache;
        }

        public async Task<DiscountDTO> Handle(UpdateDiscountCommand request, CancellationToken cancellationToken)
        {
            DiscountDTO model = request.Model;
            var id = request.Id;

            var result = _validator.Validate(model);

            _logger.LogInformation($"UpdateDiscount Validation result: {result}");

            if (!result.IsValid)
            {
                var errors = result.Errors.Select(x => x.ErrorMessage).ToArray();
                throw new InvalidRequestBodyException
                {
                    Errors = errors
                };
            }

            var dbEntity = _repository.Discounts.Get(id);

            if (dbEntity == null)
            {
                throw new EntityNotFoundException($"No Discount found for the Id {id}");
            }

            dbEntity.Amount = model.Amount;
            dbEntity.Measure = model.Measure;
            dbEntity.PromoCode = model.PromoCode;
            dbEntity.StartTime = dbEntity.StartTime;
            dbEntity.EndTime = dbEntity.EndTime;

            _repository.Discounts.Update(dbEntity);
            await _repository.CommitAsync();

            var updatedDiscount = _mapper.Map<DiscountDTO>(dbEntity);

            if (_cache.GetItem<DiscountDTO>($"discount_{id}") != null)
            {
                _logger.LogInformation($"Discount Exists in Cache. Set new Item for the same Key.");
                _cache.SetItem($"discount_{id}", updatedDiscount);
            }

            return updatedDiscount;
        }
    }
}
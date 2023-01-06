using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using POSsystem.Contracts.Data;
using POSsystem.Contracts.Data.Entities;
using POSsystem.Contracts.DTO;
using POSsystem.Contracts.Enum;
using POSsystem.Contracts.Services;
using POSsystem.Core.Exceptions;
using POSsystem.Core.Validators;

namespace POSsystem.Core.Handlers.Commands
{
    public class UpdateBranchCommand : IRequest<BranchDTO>
    {
        public CreateOrUpdateBranchDTO Model { get; }
        public int BranchId { get; set; }
        public UpdateBranchCommand(CreateOrUpdateBranchDTO model, int branchId)
        {
            this.Model = model;
            this.BranchId = branchId;

        }  
    }
    public class UpdateBranchCommandHandler : IRequestHandler<UpdateBranchCommand, BranchDTO>
    {
        private readonly IUnitOfWork _repository;
        private readonly IValidator<CreateOrUpdateBranchDTO> _validator;
        private readonly IMapper _mapper;
        private readonly ICachingService _cache;
        private readonly ILogger<UpdateBranchCommandHandler> _logger;
        
        public UpdateBranchCommandHandler(ILogger<UpdateBranchCommandHandler> logger, IUnitOfWork repository, CreateOrUpdateBranchDTOValidator validator, IMapper mapper, ICachingService cache)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
            _cache = cache;
            _logger = logger;
        }

        public void DeleteWorkingDaysByBranchId(int branchId)
        {
            var workingDays = _repository.BranchWorkingDays.GetAll();
            foreach (var day in workingDays)
            {
                if (day.BranchId == branchId)
                {
                    _repository.BranchWorkingDays.Delete(day);
                }
            }
        }
         public async Task<BranchDTO> Handle(UpdateBranchCommand request, CancellationToken cancellationToken)
         { 
             int branchId = request.BranchId;
             CreateOrUpdateBranchDTO model = request.Model;
             var result = new ValidationResult();
             try
             {
                 result = _validator.Validate(model);
             }
             catch (Exception e)
             {
                 throw new InvalidRequestBodyException
                 {
                     Errors = new string[1]{"Invalid request body"}
                 };
             }
            

            _logger.LogInformation($"UpdateBranch Validation result: {result}");

            if (!result.IsValid)
            {
                var errors = result.Errors.Select(x => x.ErrorMessage).ToArray();
                throw new InvalidRequestBodyException
                {
                    Errors = errors
                };
            }
            var dbEntity = _repository.Branches.Get(branchId);
            if(dbEntity == null)
            {
                throw new EntityNotFoundException($"No Branches found for the Id {branchId}");
            }
            DeleteWorkingDaysByBranchId(branchId);
            try
            {
                foreach (CreateOrUpdateBranchWorkingDayDTO day in model.BranchWorkingDays)
                {
                    var branchWorkingDay = new BranchWorkingDay
                    {
                        WorkingDay = (WorkingDay)day.WorkingDay,
                        WorkingHoursStart = day.WorkingHoursStart,
                        WorkingHoursEnd = day.WorkingHoursEnd,
                        BranchId = branchId
                    };
                    _repository.BranchWorkingDays.Add(branchWorkingDay); 
                    await _repository.CommitAsync();
                }
                dbEntity.Address = model.Address;
                dbEntity.Contacts = model.Contacts;
                dbEntity.BranchStatus = model.BranchStatus;
                dbEntity.CompanyId = model.CompanyId;
            }
            catch (Exception e)
            {
                var errors = result.Errors.Select(x => x.ErrorMessage).ToArray();
                throw new InvalidRequestBodyException
                {
                    Errors = new string[1]{e.ToString()}
                };
            }
            _repository.Branches.Update(dbEntity);
            await _repository.CommitAsync();

            var updatedBranch = _mapper.Map<BranchDTO>(dbEntity);

            // if a version exists in the Cache
            // replace Cached Branches with the Updated Branch
            if (_cache.GetItem<BranchDTO>($"branch_{branchId}") != null)
            {
                _logger.LogInformation($"Branch exists in Cache. Set new Branch for the same Key.");
                _cache.SetItem($"branch_{branchId}", updatedBranch);
            }

            return updatedBranch;
        }
    }
    
}


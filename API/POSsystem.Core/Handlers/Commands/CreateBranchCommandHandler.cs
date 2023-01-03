using MediatR;
using POSsystem.Contracts.Data;
using POSsystem.Contracts.DTO;
using POSsystem.Contracts.Data.Entities;
using FluentValidation;
using POSsystem.Core.Exceptions;
using AutoMapper;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using POSsystem.Contracts.Enum;
using POSsystem.Core.Validators;

namespace POSsystem.Core.Handlers.Commands
{
    public class CreateBranchCommand : IRequest<BranchDTO>
    {
        public CreateOrUpdateBranchDTO Model { get; }
        public CreateBranchCommand(CreateOrUpdateBranchDTO model)
        {
            this.Model = model;
        }
    }

    public class CreateBranchCommandHandler : IRequestHandler<CreateBranchCommand, BranchDTO>
    {
        private readonly IUnitOfWork _repository;
        private readonly IValidator<CreateOrUpdateBranchDTO> _validator;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateBranchCommandHandler> _logger;
        private readonly IDistributedCache _cache;

        public CreateBranchCommandHandler(ILogger<CreateBranchCommandHandler> logger, IUnitOfWork repository, CreateOrUpdateBranchDTOValidator validator, IMapper mapper, IDistributedCache cache)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
            _logger = logger;
            _cache = cache;
        }

        public async Task<BranchDTO> Handle(CreateBranchCommand request, CancellationToken cancellationToken)
        {
            CreateOrUpdateBranchDTO model = request.Model;

            var result = _validator.Validate(model);

            _logger.LogInformation($"CreateBranch Validation result: {result}");

            if (!result.IsValid)
            {
                var errors = result.Errors.Select(x => x.ErrorMessage).ToArray();
                throw new InvalidRequestBodyException
                {
                    Errors = errors
                };
            }

            var entity = new Branch();
            try
            {
                entity = new Branch
                {
                    Address = model.Address,
                    Contacts = model.Contacts,
                    BranchStatus = model.Status,
                    CompanyId = model.CompanyId
                };
                
                var branchResult = _repository.Branches.Add(entity); 
                await _repository.CommitAsync();
                
                foreach (CreateOrUpdateBranchWorkingDayDTO day in model.BranchWorkingDays)
                {
                    var branchWorkingDay = new BranchWorkingDay
                    {
                        WorkingDay = (WorkingDay)day.WorkingDay,
                        WorkingHoursStart = day.WorkingHoursStart,
                        WorkingHoursEnd = day.WorkingHoursEnd,
                        BranchId = branchResult.Id
                    };
                    _repository.BranchWorkingDays.Add(branchWorkingDay); 
                    await _repository.CommitAsync();
                }

            }
            catch (Exception e)
            {
                var errors = result.Errors.Select(x => x.ErrorMessage).ToArray();
                throw new InvalidRequestBodyException
                {
                    Errors = new string[1]{e.ToString()}
                };
            }
            
            _logger.LogInformation($"Updating Branch cache.");
            var updatedEntities = await Task.FromResult(_repository.Branches.GetAll());
            await _cache.SetStringAsync("all_branches", JsonConvert.SerializeObject(_mapper.Map<IEnumerable<BranchDTO>>(updatedEntities)));
            

            return _mapper.Map<BranchDTO>(entity);
        }
    }
}
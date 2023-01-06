using FluentValidation;
using MediatR;
using AutoMapper;
using Microsoft.Extensions.Logging;
using POSsystem.Contracts.Data;
using POSsystem.Contracts.Data.Entities;
using POSsystem.Core.Exceptions;
using POSsystem.Core.Handlers.Commands;
using POSsystem.Core.Validators;

namespace POSsystem.Core.Handlers.Commands
{
    public class DeleteBranchCommand : IRequest<int>
    {
        public int BranchId { get; }

        public DeleteBranchCommand(int BranchId)
        {
            BranchId = BranchId;
        }
    }
}

public class DeleteBranchCommandHandler : IRequestHandler<DeleteBranchCommand, int>
{
    private readonly IUnitOfWork _repository;
    private readonly IValidator<int> _validator;
    private readonly IMapper _mapper;
    private readonly ILogger<DeleteBranchCommandHandler> _logger;

    public DeleteBranchCommandHandler(ILogger<DeleteBranchCommandHandler> logger, IUnitOfWork repository, DeleteBranchDTOValidator validator,  IMapper mapper)
    {
        _validator = validator;
        _mapper = mapper;
        _repository = repository;
        _logger = logger;
    }

    public async Task<int> Handle(DeleteBranchCommand request, CancellationToken cancellationToken)
    {
        var result = _validator.Validate(request.BranchId);
        _logger.LogInformation($"DeleteBranch Validation result: {result}");
        
        if (!result.IsValid)
        {
            var errors = result.Errors.Select(x => x.ErrorMessage).ToArray();
            throw new InvalidRequestBodyException
            {
                Errors = errors
            };
        }
        _repository.Branches.Delete(request.BranchId);
        await _repository.CommitAsync();
        DeleteAllWorkingDays(request);
        return request.BranchId;
    }

    public void DeleteAllWorkingDays(DeleteBranchCommand request)
    {
        IEnumerable<BranchWorkingDay> workingDays = _repository.BranchWorkingDays.GetAll();
        foreach (var day in workingDays)
        {
            if (day.BranchId == request.BranchId)
            {
                _repository.BranchWorkingDays.Delete(day.Id);
            }
        }
    }
}

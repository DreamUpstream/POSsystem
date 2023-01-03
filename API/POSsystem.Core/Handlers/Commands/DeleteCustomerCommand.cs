using FluentValidation;
using MediatR;
using AutoMapper;
using Microsoft.Extensions.Logging;
using POSsystem.Contracts.Data;
using POSsystem.Core.Exceptions;
using POSsystem.Core.Handlers.Commands;
using POSsystem.Core.Validators;

namespace POSsystem.Core.Handlers.Commands
{
    public class DeleteCustomerCommand : IRequest<int>
    {
        public int CustomerId { get; }

        public DeleteCustomerCommand(int CustomerId)
        {
            this.CustomerId = CustomerId;
        }
    }
}

public class DeleteCustomerCommandHandler : IRequestHandler<DeleteCustomerCommand, int>
{
    private readonly IUnitOfWork _repository;
    private readonly IValidator<int> _validator;
    private readonly IMapper _mapper;
    private readonly ILogger<DeleteCustomerCommandHandler> _logger;

    public DeleteCustomerCommandHandler(ILogger<DeleteCustomerCommandHandler> logger, IUnitOfWork repository, DeleteCustomerDTOValidator validator,  IMapper mapper)
    {
        _validator = validator;
        _mapper = mapper;
        _repository = repository;
        _logger = logger;
    }

    public async Task<int> Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
    {
        var result = _validator.Validate(request.CustomerId);
        _logger.LogInformation($"DeleteCustomer Validation result: {result}");
        if (!result.IsValid)
        {
            var errors = result.Errors.Select(x => x.ErrorMessage).ToArray();
            throw new InvalidRequestBodyException
            {
                Errors = errors
            };
        }

        var userId = _repository.Customers.Get(request.CustomerId).UserId;
        _repository.Customers.Delete(request.CustomerId);
        _repository.Users.Delete(userId);
        await _repository.CommitAsync();
        return request.CustomerId;
    }

   
}

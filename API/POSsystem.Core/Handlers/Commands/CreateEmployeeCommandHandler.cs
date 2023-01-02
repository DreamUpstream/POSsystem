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

namespace POSsystem.Core.Handlers.Commands
{
    public class CreateEmployeeCommand : IRequest<EmployeeDTO>
    {
        public EmployeeDTO Model { get; }
        public CreateEmployeeCommand(EmployeeDTO model)
        {
            Model = model;
        }
    }

    public class CreateEmployeeCommandHandler : IRequestHandler<CreateEmployeeCommand, EmployeeDTO>
    {
        private readonly IUnitOfWork _repository;
        private readonly IValidator<EmployeeDTO> _validator;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateEmployeeCommandHandler> _logger;

        public CreateEmployeeCommandHandler(ILogger<CreateEmployeeCommandHandler> logger, IUnitOfWork repository, IValidator<EmployeeDTO> validator, IMapper mapper)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<EmployeeDTO> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
        {
            EmployeeDTO model = request.Model;

            var result = _validator.Validate(model);

            _logger.LogInformation($"CreateEmployee Validation result: {result}");

            if (!result.IsValid)
            {
                var errors = result.Errors.Select(x => x.ErrorMessage).ToArray();
                throw new InvalidRequestBodyException
                {
                    Errors = errors
                };
            }

            var entities = _repository.Users.GetAll().Where(x => x.EmailAddress == model.EmailAddress);
            if (entities.Any()) throw new DuplicateUserException($"An user with the email address {model.EmailAddress} already exists.");

            var salt = RandomNumberGenerator.GetBytes(128 / 8);
            var user = new User
            {
                EmailAddress = model.EmailAddress,
                Password = PasswordHashingService.Hash(model.Password, salt),
                Salt = salt,
                Role = UserRole.Employee
            };

            _repository.Users.Add(user);

            entities = _repository.Users.GetAll().Where(x => x.EmailAddress == model.EmailAddress);
            if (!entities.Any()) throw new EntityNotFoundException($"An user with the email address {model.EmailAddress} could not be found.");

            var entity = new Employee
            {
                Name = model.Name,
                UserId = entities.First().Id,
                RegisteredDate = model.RegisterDate,
                Status = model.Status,
                CompanyId = model.CompanyId
            };

            _repository.Employees.Add(entity);
            await _repository.CommitAsync();

            return model;
        }
    }
}
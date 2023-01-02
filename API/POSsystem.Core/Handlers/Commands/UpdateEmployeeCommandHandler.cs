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
    public class UpdateEmployeeCommand : IRequest<EmployeeDTO>
    {
        public int Id { get; }
        public EmployeeDTO Model { get; }
        public UpdateEmployeeCommand(int id, EmployeeDTO model)
        {
            Id = id;
            Model = model;
        }
    }

    public class UpdateEmployeeCommandHandler : IRequestHandler<UpdateEmployeeCommand, EmployeeDTO>
    {
        private readonly IUnitOfWork _repository;
        private readonly IValidator<EmployeeDTO> _validator;
        private readonly IMapper _mapper;
        private readonly ICachingService _cache;
        private readonly ILogger<UpdateEmployeeCommandHandler> _logger;

        public UpdateEmployeeCommandHandler(ILogger<UpdateEmployeeCommandHandler> logger, IUnitOfWork repository, IValidator<EmployeeDTO> validator, IMapper mapper, ICachingService cache)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
            _logger = logger;
            _cache = cache;
        }

        public async Task<EmployeeDTO> Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
        {
            EmployeeDTO model = request.Model;
            var id = request.Id;

            var result = _validator.Validate(model);

            _logger.LogInformation($"UpdateEmployee Validation result: {result}");

            if (!result.IsValid)
            {
                var errors = result.Errors.Select(x => x.ErrorMessage).ToArray();
                throw new InvalidRequestBodyException
                {
                    Errors = errors
                };
            }

            var user = _repository.Users.Get(id);
            if (user == null) 
            {
                throw new EntityNotFoundException($"An user with the id {id} could not be found.");
            }
            
            var employees = _repository.Employees.GetAll().Where(x => x.UserId == id);
            if(!employees.Any())
            {
                throw new EntityNotFoundException($"An employee is not registered with the email address {model.EmailAddress}.");
            }

            var employee = employees.First();

            employee.Name = String.IsNullOrEmpty(model.Name) ? employee.Name : model.Name;
            user.EmailAddress = String.IsNullOrEmpty(model.EmailAddress) ? user.EmailAddress : model.EmailAddress;
            if(!String.IsNullOrEmpty(model.Password))
            {
                var salt = RandomNumberGenerator.GetBytes(128 / 8);
                user.Password = PasswordHashingService.Hash(model.Password, salt);
                user.Salt = salt;
            }
            employee.Status = model.Status;
            employee.CompanyId = model.CompanyId;

            _repository.Users.Update(user);
            _repository.Employees.Update(employee);
            await _repository.CommitAsync();

            var updatedEmployee = _mapper.Map<EmployeeDTO>(employee);

            if (_cache.GetItem<EmployeeDTO>($"employee_{id}") != null)
            {
                _logger.LogInformation($"Employee Exists in Cache. Set new Item for the same Key.");
                _cache.SetItem($"employee_{id}", updatedEmployee);
            }

            return updatedEmployee;
        }
    }
}
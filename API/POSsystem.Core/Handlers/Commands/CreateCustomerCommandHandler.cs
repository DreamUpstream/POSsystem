using System.Security.Cryptography;
using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using POSsystem.Contracts.Data;
using POSsystem.Contracts.Data.Entities;
using POSsystem.Contracts.DTO;
using POSsystem.Contracts.Enum;
using POSsystem.Core.Exceptions;
using POSsystem.Core.Services;
using POSsystem.Core.Validators;

namespace POSsystem.Core.Handlers.Commands
{
    public class CreateCustomerCommand : IRequest<CustomerDTO>
    {
        public CreateOrUpdateCustomerDTO Model { get; }

        public CreateCustomerCommand(CreateOrUpdateCustomerDTO model)
        {
            Model = model;
        }
    }
    public class CreateCustomerCommandHandler :IRequestHandler<CreateCustomerCommand, CustomerDTO>
    {
        private readonly IUnitOfWork _repository;
        private readonly IValidator<CreateOrUpdateCustomerDTO> _validator;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateCustomerCommandHandler> _logger;
        private readonly IDistributedCache _cache;
    
        public CreateCustomerCommandHandler(ILogger<CreateCustomerCommandHandler> logger, IUnitOfWork repository, CreateOrUpdateCustomerDTOValidator validator, IMapper mapper, IDistributedCache cache)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
            _logger = logger;
            _cache = cache;
        }

        public async Task<CustomerDTO> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
        {
            CreateOrUpdateCustomerDTO model = request.Model;
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
            _logger.LogInformation($"CreateCustomer Validation result: {result}");
            handleInvalidResult(result);
            var salt = RandomNumberGenerator.GetBytes(128 / 8);
            var dbUser = new CreateOrUpdateUserDTO
            {
                EmailAddress = model.User.EmailAddress,
                Role = model.User.Role,
                Password = model.User.Password,
            };
            var newUser = new User()
            {
                EmailAddress = model.User.EmailAddress,
                Role = model.User.Role,
                Password = PasswordHashingService.Hash(model.User.Password, salt),
                Salt = salt
            };
            var userEntity = _repository.Users.Add(newUser);
            await _repository.CommitAsync();
            var dbEntity = new Customer();
            try
            {
                dbEntity = new Customer
                {
                    Name = model.Name,
                    PhoneNumber = model.PhoneNumber,
                    UserId = userEntity.Id,
                    RegisteredDate = DateTime.UtcNow,
                    Status = (CustomerStatus)model.Status,
                    DiscountId = model.DiscountId
                };
                _repository.Customers.Add(dbEntity);
                await _repository.CommitAsync();
            } catch (Exception e)
                {
                    var errors = result.Errors.Select(x => x.ErrorMessage).ToArray();
                    throw new InvalidRequestBodyException
                {
                    Errors = new string[1]{e.ToString()}
                };
            }
            _logger.LogInformation($"Updating Customer cache.");
            var updatedEntities = await Task.FromResult(_repository.Customers.GetAll());
            await _cache.SetStringAsync("all_customers", JsonConvert.SerializeObject(_mapper.Map<IEnumerable<CustomerDTO>>(updatedEntities)));
            
            return _mapper.Map<CustomerDTO>(dbEntity);
        }

        public void handleInvalidResult(ValidationResult result)
        {
            if (!result.IsValid)
            {
                var errors = result.Errors.Select(x => x.ErrorMessage).ToArray();
                throw new InvalidRequestBodyException
                {
                    Errors = errors
                };
            }
        }
    }
}
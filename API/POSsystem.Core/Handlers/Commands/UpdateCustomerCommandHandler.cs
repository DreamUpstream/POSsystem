using System.Security.Cryptography;
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
using POSsystem.Core.Services;
using POSsystem.Core.Validators;

namespace POSsystem.Core.Handlers.Commands
{
    public class UpdateCustomerCommand : IRequest<CustomerDTO>
    {
        public CreateOrUpdateCustomerDTO Model { get; }
        public int CustomerId { get; set; }
        public UpdateCustomerCommand(CreateOrUpdateCustomerDTO model, int customerId)
        {
            this.Model = model;
            this.CustomerId = customerId;

        }  
    }
    
    public class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand, CustomerDTO>
    {
        private readonly IUnitOfWork _repository;
        private readonly IValidator<CreateOrUpdateCustomerDTO> _validator;
        private readonly IMapper _mapper;
        private readonly ICachingService _cache;
        private readonly ILogger<UpdateCustomerCommandHandler> _logger;
   
        public UpdateCustomerCommandHandler(ILogger<UpdateCustomerCommandHandler> logger, IUnitOfWork repository, CreateOrUpdateCustomerDTOValidator validator, IMapper mapper, ICachingService cache)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
            _cache = cache;
            _logger = logger;
        }

        public async Task<CustomerDTO> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
        {
            int customerId = request.CustomerId;
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
            _logger.LogInformation($"UpdateBranch Validation result: {result}");

            if (!result.IsValid)
            {
                var errors = result.Errors.Select(x => x.ErrorMessage).ToArray();
                throw new InvalidRequestBodyException
                {
                    Errors = errors
                };
            }
            var dbEntity = _repository.Customers.Get(customerId);
            if(dbEntity == null)
            {
                throw new EntityNotFoundException($"No Customers found for the Id {customerId}");
            }
            var userId = _repository.Customers.Get(request.CustomerId).UserId;
            _repository.Users.Delete(userId);
            try
            {
                var salt = RandomNumberGenerator.GetBytes(128 / 8);
                var user = new User
                {
                    EmailAddress = model.User.EmailAddress,
                    Role = model.User.Role,
                    Password = PasswordHashingService.Hash(model.User.Password, salt),
                    Salt = salt
                };
                _repository.Users.Add(user); 
                await _repository.CommitAsync();
            
                dbEntity.Name = model.Name;
                dbEntity.PhoneNumber = model.PhoneNumber;
                dbEntity.RegisteredDate = _repository.Customers.Get(customerId).RegisteredDate;
                dbEntity.Status = (CustomerStatus)model.Status;
            }
            catch (Exception e)
            {
                var errors = result.Errors.Select(x => x.ErrorMessage).ToArray();
                throw new InvalidRequestBodyException
                {
                    Errors = new string[1]{e.ToString()}
                };
            }
            _repository.Customers.Update(dbEntity);
            await _repository.CommitAsync();

            var updatedCustomer = _mapper.Map<CustomerDTO>(dbEntity);
            // if a version exists in the Cache
            // replace Cached Branches with the Updated Branch
            if (_cache.GetItem<CustomerDTO>($"customer_{customerId}") != null)
            {
                _logger.LogInformation($"Customer exists in Cache. Set new Customer for the same Key.");
                _cache.SetItem($"customer_{customerId}", updatedCustomer);
            }

            return updatedCustomer;
        }
    }
}


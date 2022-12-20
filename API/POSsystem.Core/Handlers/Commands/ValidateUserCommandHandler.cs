using POSsystem.Contracts.Data;
using POSsystem.Contracts.DTO;
using POSsystem.Contracts.Services;
using POSsystem.Core.Exceptions;
using FluentValidation;
using MediatR;
using POSsystem.Core.Services;

namespace POSsystem.Core.Handlers.Commands
{
    public class ValidateUserCommand : IRequest<AuthTokenDTO>
    {
        public ValidateUserCommand(ValidateUserDTO model)
        {
            Model = model;
        }

        public ValidateUserDTO Model { get; }
    }

    public class ValidateUserCommandHandler : IRequestHandler<ValidateUserCommand, AuthTokenDTO>
    {
        private readonly IUnitOfWork _repository;
        private readonly IValidator<ValidateUserDTO> _validator;
        private readonly ITokenService _token;

        public ValidateUserCommandHandler(IUnitOfWork repository, IValidator<ValidateUserDTO> validator, ITokenService token)
        {
            _repository = repository;
            _validator = validator;
            _token = token;
        }

        public async Task<AuthTokenDTO> Handle(ValidateUserCommand request, CancellationToken cancellationToken)
        {
            var model = request.Model;

            var result = _validator.Validate(model);

            if (!result.IsValid)
            {
                var errors = result.Errors.Select(x => x.ErrorMessage).ToArray();
                throw new InvalidRequestBodyException
                {
                    Errors = errors
                };
            }

            var entities = _repository.Users.GetAll().Where(x => x.EmailAddress == model.EmailAddress);
            if (!entities.Any()) throw new EntityNotFoundException($"No Users matching emailAddress {model.EmailAddress} found");

            var user = entities.Where(x => x.Password == PasswordHashingService.Hash(model.Password, x.Salt)).FirstOrDefault();
            if(user == null) throw new EntityNotFoundException($"Passwords do not match. Authentication Failed.");

            return await Task.FromResult(_token.Generate(user));
        }
    }
}

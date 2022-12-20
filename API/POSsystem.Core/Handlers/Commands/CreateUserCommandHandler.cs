using System.Security.Cryptography;
using POSsystem.Contracts.Data;
using POSsystem.Contracts.Data.Entities;
using POSsystem.Contracts.DTO;
using POSsystem.Contracts.Services;
using POSsystem.Core.Exceptions;
using FluentValidation;
using MediatR;
using POSsystem.Core.Services;

namespace POSsystem.Core.Handlers.Commands
{
    public class CreateUserCommand : IRequest<AuthTokenDTO>
    {
        public CreateUserCommand(CreateOrUpdateUserDTO model)
        {
            Model = model;
        }

        public CreateOrUpdateUserDTO Model { get; }
    }

    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, AuthTokenDTO>
    {
        private readonly IUnitOfWork _repository;
        private readonly IValidator<CreateOrUpdateUserDTO> _validator;
        private readonly ITokenService _token;

        public CreateUserCommandHandler(IUnitOfWork repository, IValidator<CreateOrUpdateUserDTO> validator, ITokenService token)
        {
            _repository = repository;
            _validator = validator;
            _token = token;
        }

        public async Task<AuthTokenDTO> Handle(CreateUserCommand request, CancellationToken cancellationToken)
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

            var salt = RandomNumberGenerator.GetBytes(128 / 8);
            var user = new User
            {
                EmailAddress = model.EmailAddress,
                Password = PasswordHashingService.Hash(model.Password, salt),
                Salt = salt,
                Role = model.Role
            };

            _repository.Users.Add(user);
            await _repository.CommitAsync();

            return _token.Generate(user);
        }
    }
}

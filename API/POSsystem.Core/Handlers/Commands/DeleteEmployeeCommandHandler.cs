using MediatR;
using POSsystem.Contracts.Data;

namespace POSsystem.Core.Handlers.Commands
{
    public class DeleteEmployeeCommand : IRequest<int>
    {
        public int EmployeeId { get; }

        public DeleteEmployeeCommand(int itemId)
        {
            EmployeeId = itemId;
        }
    }

    public class DeleteEmployeeCommandHandler : IRequestHandler<DeleteEmployeeCommand, int>
    {
        private readonly IUnitOfWork _repository;

        public DeleteEmployeeCommandHandler(IUnitOfWork repository)
        {
            _repository = repository;
        }

        public async Task<int> Handle(DeleteEmployeeCommand request, CancellationToken cancellationToken)
        {
            _repository.Employees.Delete(request.EmployeeId);
            await _repository.CommitAsync();
            return request.EmployeeId;
        }
    }
}
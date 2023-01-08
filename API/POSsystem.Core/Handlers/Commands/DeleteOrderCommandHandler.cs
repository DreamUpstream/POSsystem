using MediatR;
using POSsystem.Contracts.Data;

namespace POSsystem.Core.Handlers.Commands
{
    public class DeleteOrderCommand : IRequest<int>
    {
        public int OrderId { get; }

        public DeleteOrderCommand(int id)
        {
            OrderId = id;
        }
    }

    public class DeleteOrderCommandHandler : IRequestHandler<DeleteOrderCommand, int>
    {
        private readonly IUnitOfWork _repository;

        public DeleteOrderCommandHandler(IUnitOfWork repository)
        {
            _repository = repository;
        }

        public async Task<int> Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
        {
            _repository.Orders.Delete(request.OrderId);
            await _repository.CommitAsync();
            return request.OrderId;
        }
    }
}
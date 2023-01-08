using MediatR;
using POSsystem.Contracts.Data;

namespace POSsystem.Core.Handlers.Commands
{
    public class DeleteDiscountCommand : IRequest<int>
    {
        public int DiscountId { get; }

        public DeleteDiscountCommand(int id)
        {
            DiscountId = id;
        }
    }

    public class DeleteDiscountCommandHandler : IRequestHandler<DeleteDiscountCommand, int>
    {
        private readonly IUnitOfWork _repository;

        public DeleteDiscountCommandHandler(IUnitOfWork repository)
        {
            _repository = repository;
        }

        public async Task<int> Handle(DeleteDiscountCommand request, CancellationToken cancellationToken)
        {
            _repository.Discounts.Delete(request.DiscountId);
            await _repository.CommitAsync();
            return request.DiscountId;
        }
    }
}
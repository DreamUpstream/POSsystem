using MediatR;
using POSsystem.Contracts.Data;

namespace POSsystem.Core.Handlers.Commands;

public class DeleteServiceReservationCommand : IRequest<int>
{
    public int ServiceReservationId { get; }

    public DeleteServiceReservationCommand(int serviceReservationId)
    {
        ServiceReservationId = serviceReservationId;
    }
}

public class DeleteServiceReservationCommandHandler : IRequestHandler<DeleteServiceReservationCommand, int>
{
    private readonly IUnitOfWork _repository;

    public DeleteServiceReservationCommandHandler(IUnitOfWork repository)
    {
        _repository = repository;
    }

    public async Task<int> Handle(DeleteServiceReservationCommand request, CancellationToken cancellationToken)
    {
        _repository.ServiceReservations.Delete(request.ServiceReservationId);
        await _repository.CommitAsync();
        return request.ServiceReservationId;
    }
}
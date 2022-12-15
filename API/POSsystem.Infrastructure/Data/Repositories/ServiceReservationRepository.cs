using POSsystem.Contracts.Data.Entities;
using POSsystem.Contracts.Data.Repositories;
using POSsystem.Migrations;

namespace POSsystem.Core.Data.Repositories;

public class ServiceReservationRepository : Repository<ServiceReservation>, IServiceReservationRepository
{
    public ServiceReservationRepository(DatabaseContext context) : base(context)
    {
        
    }
}
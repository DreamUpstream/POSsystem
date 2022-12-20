using POSsystem.Contracts.Enum;

namespace POSsystem.Contracts.Data.Entities;

public class ServiceReservation : AuditableEntity
{
    public DateTime Time { get; set; }
    public ReservationStatus Status { get; set; }
    public Service Service { get; set; }
    public long TaxId { get; set; }
    public Order Order { get; set; }
    public Employee Employee { get; set; }
}
using POSsystem.Contracts.Enum;

namespace POSsystem.Contracts.DTO;

public class CreateOrUpdateServiceReservationDTO
{
    public DateTime Time { get; set; }
    public ReservationStatus ReservationStatus { get; set; }
    public int ServiceId { get; set; }
    public int TaxId { get; set; }
    public int OrderId { get; set; }
    public int EmployeeId { get; set; }
}
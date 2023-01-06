using System.ComponentModel.DataAnnotations.Schema;
using POSsystem.Contracts.Enum;

namespace POSsystem.Contracts.Data.Entities
{
    public class ServiceReservation : AuditableEntity
    {
        public string Time { get; set; }
        public ReservationStatus ReservationStatus { get; set; }
        [ForeignKey("services")]
        public int ServiceId { get; set; }
        public int TaxId { get; set; }
        [ForeignKey("orders")]
        public int OrderId { get; set; }
        [ForeignKey("employees")]
        public int EmployeeId { get; set; }
    }
}
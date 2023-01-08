using POSsystem.Contracts.Enum;

namespace POSsystem.Contracts.DTO
{
    public class EmployeeDTO
    {
        public string Name { get; set; }
        public int UserId { get; set; }
        public DateTime RegisterDate { get; set; }
        public EmployeeStatus Status { get; set; }
        public int CompanyId { get; set; }
    }
}
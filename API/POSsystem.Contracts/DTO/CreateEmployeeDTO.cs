using POSsystem.Contracts.Enum;

namespace POSsystem.Contracts.DTO
{
    public class CreateEmployeeDTO
    {
        public string Name { get; set; }
        public string EmailAddress { get; set; }
        public string Password { get; set; }
        public DateTime RegisterDate { get; set; }
        public EmployeeStatus Status { get; set; }
        public int CompanyId { get; set; }
    }
}
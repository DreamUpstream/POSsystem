namespace POSsystem.Contracts.Data.Entities;

// There is not Company entity in the swagger, but I've added it on my own
public class Company : AuditableEntity
{
    public string Name { get; set; }
}
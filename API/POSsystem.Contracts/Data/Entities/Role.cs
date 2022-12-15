namespace POSsystem.Contracts.Data.Entities;

// employment role, probably should rename to something less similar to UserRole
public class Role : AuditableEntity
{
    public string Name { get; set; }
}
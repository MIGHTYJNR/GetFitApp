namespace GetFitApp.Data.Entities;

public class Benefit : BaseEntity
{
    public Guid MembershipTypeId { get; set; }
    public MembershipType MembershipType { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
}
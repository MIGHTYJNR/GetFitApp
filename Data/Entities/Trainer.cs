using GetFitApp.Data.Enums;

namespace GetFitApp.Data.Entities;

public class Trainer : BaseEntity
{
    public string UserId { get; set; } = default!;
    public User User { get; set; } = default!;
    public Guid SpecializationId { get; set; }
    public Specialization Specialization { get; set; } = default!;
    public string Firstname { get; set; } = default!;
    public string Lastname { get; set; } = default!;
    public string Middlename { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string PhoneNumber { get; set; } = default!;
    public int Age { get; set; }
    public Gender Gender { get; set; } = default!;
    public string Address { get; set; } = default!;
    public ICollection<Member> Members { get; set; } = [];
}
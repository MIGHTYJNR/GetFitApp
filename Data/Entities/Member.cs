using GetFitApp.Data.Enums;

namespace GetFitApp.Data.Entities;

public class Member : BaseEntity
{
    public string UserId { get; set; } = default!;
    public User User { get; set; } = default!;
    public Guid MembershipTypeId { get; set; }
    public MembershipType MembershipType { get; set; } = default!;
    public Guid TrainerId { get; set; }
    public Trainer PreferredTrainer { get; set; } = default!;
    public Guid FitnessClassId { get; set; }
    public FitnessClass FitnessClass { get; set; } = default!;
    public string Firstname { get; set; } = default!;
    public string Lastname { get; set; } = default!;
    public string Middlename { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string PhoneNumber { get; set; } = default!;
    public int Age { get; set; }
    public Gender Gender { get; set; } = default!;
    public string Address { get; set; } = default!;
    public string EmergencyContact { get; set; } = default!;
    public string FitnessGoal { get; set; } = default!;
    public DateTime ExpiryDate { get; set; }
}

using GetFitApp.Data.Enums;

namespace GetFitApp.Data.Entities
{
    public class MembershipType : BaseEntity
    {
        public string MembershipTypeName { get; set; } = default!;
        public DurationType Duration { get; set; }
        public decimal Amount { get; set; }
        public ICollection<Benefit> Benefits { get; set; } = default!;
        public ICollection<Member> Members { get; set; } = default!;
    }
}

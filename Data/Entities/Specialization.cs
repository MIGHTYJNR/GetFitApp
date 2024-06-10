namespace GetFitApp.Data.Entities
{
    public class Specialization : BaseEntity
    {
        public string SpecializationName { get; set; } = default!;
        public bool IsAvailable { get; set; }
    }
}

namespace GetFitApp.Models.FitnessClass
{
    public class FitnessClassDisplayViewModel
    {
        public string Name { get; set; } = default!;
        public string Schedule { get; set; } = default!;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
    }
}

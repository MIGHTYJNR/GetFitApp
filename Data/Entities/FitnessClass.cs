﻿using GetFitApp.Data.Entities;

namespace GetFitApp.Data.Entities
{
    public class FitnessClass : BaseEntity
    {
        public string Name { get; set; } = default!;
        public string Schedule { get; set; } = default!;
        public int Duration { get; set; }
        public string? ImageUrl { get; set; }
        public string? Description { get; set; }

        public Guid TrainerId { get; set; }
        public Trainer Trainer { get; set; } = default!;

        public ICollection<Member> Members { get; set; } = default!;
    }

}


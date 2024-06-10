using GetFitApp.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace GetFitApp.Data
{
    public class GetFitContext(DbContextOptions<GetFitContext> options) : IdentityDbContext(options)
    {
        protected override void OnModelCreating(ModelBuilder builder)
        {
            //builder.Entity<MembershipType>()
            //    .Property(s => s.Benefits)
            //    .HasConversion(new ValueConverter<List<string>, string>(
            //        v => JsonConvert.SerializeObject(v),
            //        v => JsonConvert.DeserializeObject<List<string>>(v)));

            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

        public DbSet<Member> MemberDetails { get; set; }
        public DbSet<MembershipType> MembershipTypes { get; set; }
        public DbSet<Trainer> Trainers { get; set; }
        public DbSet<FitnessClass> FitnessClasses { get; set; }
        public DbSet<Specialization> Specializations { get; set; }
        public DbSet<Benefit> Benefits { get; set; }
        /*public DbSet<MemberClass> MemberClasses { get; set; }
        public DbSet<Payment> Payments { get; set; }*/
    }
}

using APPR_POE_PART_1_prototype.Models;
using GiftOfTheGivers;
using Microsoft.EntityFrameworkCore;

namespace APPR_POE_PART_1_prototype.Data
{
    public class GOGAppDBContext : DbContext
    {
        public GOGAppDBContext(DbContextOptions<GOGAppDBContext> options): base(options)
        {
        }
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Volunteer> Volunteers { get; set; } = null!;
        public DbSet<DonationRecord> Donations { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Volunteer>()
                .HasOne(v => v.User)
                .WithOne()
                .HasForeignKey<Volunteer>(v => v.UserID);

            modelBuilder.Entity<DonationRecord>()
                .Property(d => d.Amount)
                .HasPrecision(18, 2);
        }
    }
}

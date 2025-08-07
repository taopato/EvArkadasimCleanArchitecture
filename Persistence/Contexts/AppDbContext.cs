using Microsoft.EntityFrameworkCore;
using Domain.Entities;

namespace Persistence.Contexts
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> opts)
            : base(opts) { }

        public DbSet<Expense> Expenses { get; set; }
        public DbSet<House> Houses { get; set; }
        public DbSet<HouseMember> HouseMembers { get; set; }
        public DbSet<Invitation> Invitations { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<PersonalExpense> PersonalExpenses { get; set; }
        public DbSet<Share> Shares { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<VerificationCode> VerificationCodes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Expense: precision and restrict cascade for two user FKs
            modelBuilder.Entity<Expense>(eb =>
            {
                eb.Property(e => e.Tutar)
                  .HasPrecision(18, 2);
                eb.Property(e => e.OrtakHarcamaTutari)
                  .HasPrecision(18, 2);

                eb.HasOne(e => e.OdeyenUser)
                  .WithMany()
                  .HasForeignKey(e => e.OdeyenUserId)
                  .OnDelete(DeleteBehavior.Restrict);

                eb.HasOne(e => e.KaydedenUser)
                  .WithMany()
                  .HasForeignKey(e => e.KaydedenUserId)
                  .OnDelete(DeleteBehavior.Restrict);
            });

            // Payment: precision and restrict cascade
            modelBuilder.Entity<Payment>(pb =>
            {
                pb.Property(p => p.Tutar)
                  .HasPrecision(18, 2);

                pb.HasOne(p => p.BorcluUser)
                  .WithMany()
                  .HasForeignKey(p => p.BorcluUserId)
                  .OnDelete(DeleteBehavior.Restrict);

                pb.HasOne(p => p.AlacakliUser)
                  .WithMany()
                  .HasForeignKey(p => p.AlacakliUserId)
                  .OnDelete(DeleteBehavior.Restrict);
            });

            // PersonalExpense: precision
            modelBuilder.Entity<PersonalExpense>(peb =>
            {
                peb.Property(pe => pe.Tutar)
                   .HasPrecision(18, 2);
            });

            // Share: precision
            modelBuilder.Entity<Share>(sb =>
            {
                sb.Property(s => s.PaylasimTutar)
                  .HasPrecision(18, 2);
            });

            modelBuilder.Entity<HouseMember>()
    .HasKey(hm => new { hm.HouseId, hm.UserId });

            modelBuilder.Entity<House>()
    .HasOne(h => h.CreatorUser)
    .WithMany()
    .HasForeignKey(h => h.CreatorUserId)
    .OnDelete(DeleteBehavior.Restrict);



            modelBuilder.Entity<Expense>()
    .HasOne(e => e.OdeyenUser)
    .WithMany()
    .HasForeignKey(e => e.OdeyenUserId)
    .OnDelete(DeleteBehavior.Restrict);

        }
    }
}

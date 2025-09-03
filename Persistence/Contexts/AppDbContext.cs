using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Domain.Enums;
using Persistence.Configurations; // <-- eklendi

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
        public DbSet<Payment> Payments { get; set; } = null!;
        public DbSet<PersonalExpense> PersonalExpenses { get; set; }
        public DbSet<Share> Shares { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<VerificationCode> VerificationCodes { get; set; }
        public DbSet<Bill> Bills { get; set; }
        public DbSet<BillShare> BillShares { get; set; }
        public DbSet<BillDocument> BillDocuments { get; set; }
        public DbSet<LedgerEntry> LedgerEntries { get; set; }
        public DbSet<PaymentAllocation> PaymentAllocations { get; set; }

        public DbSet<RecurringCharge> RecurringCharges { get; set; } = null!;
        public DbSet<ChargeCycle> ChargeCycles { get; set; } = null!;

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

                // Enum int olarak saklanır, default BankTransfer
                pb.Property(p => p.PaymentMethod)
                  .HasConversion<int>()
                  .HasDefaultValue(PaymentMethod.BankTransfer);

                // YENİ: PaymentStatus enum mapping
                pb.Property(p => p.Status)
                  .HasConversion<int>()
                  .HasDefaultValue(PaymentStatus.Pending);

                // DekontUrl non-nullable kalıyor; sadece max length belirledik
                pb.Property(p => p.DekontUrl)
                  .HasMaxLength(400);

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

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.BorcluUser)
                .WithMany()
                .HasForeignKey(p => p.BorcluUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.AlacakliUser)
                .WithMany()
                .HasForeignKey(p => p.AlacakliUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Payment>(eb =>
            {
                eb.Property(p => p.AlacakliOnayi)
                  .HasColumnType("bit")
                  .HasDefaultValue(false)
                  .IsRequired();
            });

            modelBuilder.Entity<Bill>(eb =>
            {
                eb.Property(x => x.Amount).HasColumnType("decimal(18,2)");
                eb.Property(x => x.Month).HasMaxLength(7).IsRequired(); // "YYYY-MM"
                eb.HasIndex(x => new { x.HouseId, x.UtilityType, x.Month }).IsUnique();

                eb.HasMany(x => x.Shares)
                  .WithOne(s => s.Bill)
                  .HasForeignKey(s => s.BillId)
                  .OnDelete(DeleteBehavior.Cascade);

                eb.HasMany(x => x.Documents)
                  .WithOne(d => d.Bill)
                  .HasForeignKey(d => d.BillId)
                  .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<BillShare>(eb =>
            {
                eb.Property(x => x.ShareAmount).HasColumnType("decimal(18,2)");
            });

            modelBuilder.Entity<BillDocument>(eb =>
            {
                eb.Property(x => x.FileName).HasMaxLength(260);
                eb.Property(x => x.FilePathOrUrl).HasMaxLength(1024);
            });

            modelBuilder.Entity<LedgerEntry>(le =>
            {
                le.Property(x => x.Amount).HasColumnType("decimal(18,2)");
                le.Property(x => x.PaidAmount).HasColumnType("decimal(18,2)");
                le.HasIndex(x => new { x.HouseId, x.FromUserId, x.ToUserId, x.CreatedAt });
            });

            modelBuilder.Entity<PaymentAllocation>(pa =>
            {
                pa.Property(x => x.Amount).HasColumnType("decimal(18,2)");
                // Payment tablonuzun adı "Payments" ise:
                pa.HasIndex(x => x.PaymentId);
                pa.HasIndex(x => x.LedgerEntryId);
            });
            ///////////////////////////////
            modelBuilder.Entity<RecurringCharge>(b =>
            {
                b.HasKey(x => x.Id);
                b.Property(x => x.Type).HasConversion<int>();
                b.Property(x => x.AmountMode).HasConversion<int>();
                b.Property(x => x.SplitPolicy).HasConversion<int>();
                b.Property(x => x.FixedAmount).HasPrecision(18, 2);   // ⬅️ eklendi

            });

            modelBuilder.Entity<ChargeCycle>(b =>
            {
                b.HasKey(x => x.Id);
                b.Property(x => x.Status).HasConversion<int>();
                b.HasIndex(x => new { x.ContractId, x.Period }).IsUnique();
                b.Property(x => x.TotalAmount).HasPrecision(18, 2);   // ⬅️ eklendi
                b.Property(x => x.FundedAmount).HasPrecision(18, 2);  // ⬅️ eklendi
            });

            modelBuilder.Entity<Payment>(b =>
            {
                b.HasOne(p => p.Charge)
                 .WithMany()
                 .HasForeignKey(p => p.ChargeId)
                 .OnDelete(DeleteBehavior.Restrict);
            });
            ///////////////////////////////
            modelBuilder.ApplyConfiguration(new ExpenseConfiguration());
            modelBuilder.ApplyConfiguration(new LedgerLineConfiguration());


        }
    }
}

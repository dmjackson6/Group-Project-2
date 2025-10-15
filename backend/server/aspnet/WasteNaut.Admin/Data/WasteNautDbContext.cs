using Microsoft.EntityFrameworkCore;
using WasteNaut.Admin.Models;

namespace WasteNaut.Admin.Data
{
    public class WasteNautDbContext : DbContext
    {
        public WasteNautDbContext(DbContextOptions<WasteNautDbContext> options) : base(options)
        {
        }

        // DbSets for all entities
        public DbSet<Admin> Admins { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<Donation> Donations { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<Match> Matches { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<ReportEvidence> ReportEvidence { get; set; }
        public DbSet<ReportNote> ReportNotes { get; set; }
        public DbSet<MatchNote> MatchNotes { get; set; }
        public DbSet<NotificationTemplate> NotificationTemplates { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships and constraints
            modelBuilder.Entity<Admin>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasOne(e => e.Role)
                      .WithMany(r => r.Admins)
                      .HasForeignKey(e => e.RoleId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasOne(e => e.Organization)
                      .WithMany(o => o.Users)
                      .HasForeignKey(e => e.OrganizationId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<Organization>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.RegistrationNumber).IsUnique();
            });

            modelBuilder.Entity<Donation>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Donor)
                      .WithMany()
                      .HasForeignKey(e => e.DonorId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Recipient)
                      .WithMany()
                      .HasForeignKey(e => e.RecipientId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<Report>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Reporter)
                      .WithMany()
                      .HasForeignKey(e => e.ReporterId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.ReportedUser)
                      .WithMany()
                      .HasForeignKey(e => e.ReportedUserId)
                      .OnDelete(DeleteBehavior.SetNull);
                entity.HasOne(e => e.AssignedAdmin)
                      .WithMany()
                      .HasForeignKey(e => e.AssignedAdminId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<Match>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.FromUser)
                      .WithMany()
                      .HasForeignKey(e => e.FromUserId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.ToUser)
                      .WithMany()
                      .HasForeignKey(e => e.ToUserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure JSON columns for MySQL
            modelBuilder.Entity<User>()
                .Property(e => e.Preferences)
                .HasConversion(
                    v => v ?? string.Empty,
                    v => v
                );

            modelBuilder.Entity<Organization>()
                .Property(e => e.ServiceAreas)
                .HasConversion(
                    v => v ?? string.Empty,
                    v => v
                );

            modelBuilder.Entity<Donation>()
                .Property(e => e.Tags)
                .HasConversion(
                    v => v ?? string.Empty,
                    v => v
                );

            modelBuilder.Entity<Match>()
                .Property(e => e.Factors)
                .HasConversion(
                    v => v ?? string.Empty,
                    v => v
                );

            modelBuilder.Entity<Role>()
                .Property(e => e.Permissions)
                .HasConversion(
                    v => v ?? string.Empty,
                    v => v
                );
        }
    }
}

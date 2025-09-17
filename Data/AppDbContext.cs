using Microsoft.EntityFrameworkCore;
using CMCS.Web.Models;

namespace CMCS.Web.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Lecturer> Lecturers { get; set; }
        public DbSet<Claim> Claims { get; set; }
        public DbSet<ClaimItem> ClaimItems { get; set; }
        public DbSet<SupportingDocument> SupportingDocuments { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // relationships
            modelBuilder.Entity<UserRole>().HasMany(r => r.Users).WithOne(u => u.Role).HasForeignKey(u => u.RoleId);

            modelBuilder.Entity<User>()
                .HasOne(u => u.LecturerProfile)
                .WithOne(l => l.User)
                .HasForeignKey<Lecturer>(l => l.UserId);

            modelBuilder.Entity<Lecturer>().HasMany(l => l.Claims).WithOne(c => c.Lecturer).HasForeignKey(c => c.LecturerId);

            modelBuilder.Entity<Claim>().HasMany(c => c.ClaimItems).WithOne(ci => ci.Claim).HasForeignKey(ci => ci.ClaimId);
            modelBuilder.Entity<Claim>().HasMany(c => c.SupportingDocuments).WithOne(sd => sd.Claim).HasForeignKey(sd => sd.ClaimId);
            modelBuilder.Entity<Claim>().HasMany(c => c.AuditLogs).WithOne(al => al.Claim).HasForeignKey(al => al.ClaimId);

            // decimal types already specified via attributes above
            base.OnModelCreating(modelBuilder);
        }
    }
}

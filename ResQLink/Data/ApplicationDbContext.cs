using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ResQLink.Models;

namespace ResQLink.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Profile> Profiles { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Fix for SQLite compatibility - use TEXT instead of nvarchar(max)
            builder.Entity<IdentityRole>(b =>
            {
                b.Property(r => r.ConcurrencyStamp).HasColumnType("TEXT");
                b.Property(r => r.Name).HasColumnType("TEXT");
                b.Property(r => r.NormalizedName).HasColumnType("TEXT");
            });

            builder.Entity<ApplicationUser>(b =>
            {
                b.Property(u => u.ConcurrencyStamp).HasColumnType("TEXT");
                b.Property(u => u.SecurityStamp).HasColumnType("TEXT");
                b.Property(u => u.PhoneNumber).HasColumnType("TEXT");
                b.Property(u => u.Email).HasColumnType("TEXT");
                b.Property(u => u.NormalizedEmail).HasColumnType("TEXT");
                b.Property(u => u.UserName).HasColumnType("TEXT");
                b.Property(u => u.NormalizedUserName).HasColumnType("TEXT");
            });

            builder.Entity<IdentityUserLogin<string>>(b =>
            {
                b.Property(l => l.ProviderKey).HasColumnType("TEXT");
                b.Property(l => l.LoginProvider).HasColumnType("TEXT");
            });

            builder.Entity<IdentityUserToken<string>>(b =>
            {
                b.Property(t => t.LoginProvider).HasColumnType("TEXT");
                b.Property(t => t.Name).HasColumnType("TEXT");
                b.Property(t => t.Value).HasColumnType("TEXT");
            });

            builder.Entity<IdentityUserClaim<string>>(b =>
            {
                b.Property(c => c.ClaimType).HasColumnType("TEXT");
                b.Property(c => c.ClaimValue).HasColumnType("TEXT");
            });

            builder.Entity<IdentityRoleClaim<string>>(b =>
            {
                b.Property(c => c.ClaimType).HasColumnType("TEXT");
                b.Property(c => c.ClaimValue).HasColumnType("TEXT");
            });
        }
    }
}

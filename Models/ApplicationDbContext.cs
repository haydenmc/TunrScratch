using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Tunr.Models.Library;

namespace Tunr.Models
{
    public class ApplicationDbContext: IdentityDbContext<TunrUser, TunrRole, Guid>
    {
        public DbSet<Track> Tracks { get; set; }
        
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options)
        {
            // This space intentionally left blank.
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Fix up Identity table names
            builder.Entity<TunrUser>().ToTable("Users");
            builder.Entity<TunrRole>().ToTable("Roles");
            builder.Entity<IdentityUserToken<Guid>>().ToTable("UserTokens");
            builder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaims");
            builder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims");
            builder.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogins");
            builder.Entity<IdentityUserRole<Guid>>().ToTable("UserRoles");

            // Set up keys and such
            builder.Entity<TunrUser>().HasKey(u => u.Id).ForSqlServerIsClustered(false);
            builder.Entity<Track>().HasKey(k => k.TrackId).ForSqlServerIsClustered(false);
            builder.Entity<Track>().HasOne<TunrUser>().WithMany().IsRequired(true);
        }
    }
}
using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Tunr.Models.Library;

namespace Tunr.Models
{
    public class ApplicationDbContext: IdentityDbContext<TunrUser, TunrRole, Guid>
    {
        public DbSet<Track> Tracks { get; set; }

        public DbSet<TrackPerformer> TrackPerformers { get; set; }

        public DbSet<TrackComposer> TrackComposers { get; set; }

        public DbSet<TrackGenre> TrackGenres { get; set; }

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

            // Set up keys and relationships
            // User key
            builder.Entity<TunrUser>().HasKey(u => u.Id).ForSqlServerIsClustered(false);
            builder.Entity<TunrUser>().HasKey(u => u.UserName).ForSqlServerIsClustered(true);

            // Role key
            builder.Entity<TunrRole>().HasKey(r => r.Id).ForSqlServerIsClustered(false);
            builder.Entity<TunrRole>().HasKey(r => r.Name).ForSqlServerIsClustered(true);

            // Track key
            builder.Entity<Track>().HasKey(t => t.TrackId).ForSqlServerIsClustered(false);
            builder.Entity<Track>().HasKey(t => t.TagTitle).ForSqlServerIsClustered(true);

            // Role <-> IdentityRoleClaim
            builder.Entity<TunrRole>()
                .HasMany<IdentityRoleClaim<Guid>>()
                .WithOne()
                .HasForeignKey(rc => rc.RoleId)
                .HasPrincipalKey(r => r.Id);

            // Role <-> IdentityUserRole
            builder.Entity<TunrRole>()
                .HasMany<IdentityUserRole<Guid>>()
                .WithOne()
                .HasForeignKey(ur => ur.RoleId)
                .HasPrincipalKey(r => r.Id);

            // User <-> IdentityUserRole
            builder.Entity<TunrUser>()
                .HasMany<IdentityUserRole<Guid>>()
                .WithOne()
                .HasForeignKey(ur => ur.UserId)
                .HasPrincipalKey(u => u.Id);

            // User <-> IdentityUserClaim
            builder.Entity<TunrUser>()
                .HasMany<IdentityUserClaim<Guid>>()
                .WithOne()
                .HasForeignKey(uc => uc.UserId)
                .HasPrincipalKey(u => u.Id);

            // User <-> IdentityUserLogin
            builder.Entity<TunrUser>()
                .HasMany<IdentityUserLogin<Guid>>()
                .WithOne()
                .HasForeignKey(ul => ul.UserId)
                .HasPrincipalKey(u => u.Id);

            // User <-> IdentityUserToken
            builder.Entity<TunrUser>()
                .HasMany<IdentityUserToken<Guid>>()
                .WithOne()
                .HasForeignKey(ut => ut.UserId)
                .HasPrincipalKey(u => u.Id);

            // Track <-> User
            builder.Entity<Track>()
                .HasOne<TunrUser>()
                .WithMany()
                .HasForeignKey(t => t.UserId)
                .HasPrincipalKey(u => u.Id)
                .IsRequired(true);
            // Track ignored props
            builder.Entity<Track>()
                .Ignore(t => t.TagPerformers)
                .Ignore(t => t.TagComposers)
                .Ignore(t => t.TagGenres);

            // TrackPerformer key
            builder.Entity<TrackPerformer>()
                .HasKey(tp => new { tp.TrackId, tp.Performer });
            // TrackPerformer <-> Track
            builder.Entity<TrackPerformer>()
                .HasOne<Track>(tp => tp.Track)
                .WithMany(t => t.DbTagPerformers)
                .HasForeignKey(tp => tp.TrackId)
                .HasPrincipalKey(t => t.TrackId)
                .IsRequired(true);

            // TrackComposer key
            builder.Entity<TrackComposer>()
                .HasKey(tc => new { tc.TrackId, tc.Composer });
            // TrackComposer <-> Track
            builder.Entity<TrackComposer>()
                .HasOne<Track>(tc => tc.Track)
                .WithMany(t => t.DbTagComposers)
                .HasForeignKey(tc => tc.TrackId)
                .HasPrincipalKey(t => t.TrackId)
                .IsRequired(true);

            // TrackGenre key
            builder.Entity<TrackGenre>()
                .HasKey(tg => new { tg.TrackId, tg.Genre });
            // TrackGenre <-> Track
            builder.Entity<TrackGenre>()
                .HasOne<Track>(tg => tg.Track)
                .WithMany(t => t.DbTagGenres)
                .HasForeignKey(tg => tg.TrackId)
                .HasPrincipalKey(t => t.TrackId)
                .IsRequired(true);
        }
    }
}
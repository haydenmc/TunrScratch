using System;
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

            // Track key
            builder.Entity<Track>().HasKey(k => k.TrackId).ForSqlServerIsClustered(false);
            // Track <-> User
            builder.Entity<Track>()
                .HasOne<TunrUser>()
                .WithMany()
                .HasForeignKey(t => t.UserId)
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
                .IsRequired(true);

            // TrackComposer key
            builder.Entity<TrackComposer>()
                .HasKey(tc => new { tc.TrackId, tc.Composer });
            // TrackComposer <-> Track
            builder.Entity<TrackComposer>()
                .HasOne<Track>(tc => tc.Track)
                .WithMany(t => t.DbTagComposers)
                .HasForeignKey(tc => tc.TrackId)
                .IsRequired(true);

            // TrackGenre key
            builder.Entity<TrackGenre>()
                .HasKey(tg => new { tg.TrackId, tg.Genre });
            // TrackGenre <-> Track
            builder.Entity<TrackGenre>()
                .HasOne<Track>(tg => tg.Track)
                .WithMany(t => t.DbTagGenres)
                .HasForeignKey(tg => tg.TrackId)
                .IsRequired(true);
        }
    }
}
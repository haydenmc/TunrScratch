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

            // TrackPerformer key
            builder.Entity<TrackPerformer>()
                .HasKey(tp => new { tp.TrackId, tp.Performer });
            // Track <-> TrackPerformer
            builder.Entity<Track>()
                .HasMany<TrackPerformer>()
                .WithOne(tp => tp.Track)
                .HasForeignKey(tp => tp.TrackId)
                .OnDelete(DeleteBehavior.Cascade);

            // TrackComposer key
            builder.Entity<TrackComposer>()
                .HasKey(tc => new { tc.TrackId, tc.Composer });
            // Track <-> TrackComposer
            builder.Entity<Track>()
                .HasMany<TrackComposer>()
                .WithOne(tc => tc.Track)
                .HasForeignKey(tc => tc.TrackId)
                .OnDelete(DeleteBehavior.Cascade);

            // TrackGenre key
            builder.Entity<TrackGenre>()
                .HasKey(tg => new { tg.TrackId, tg.Genre });
            // Track <-> TrackGenre
            builder.Entity<Track>()
                .HasMany<TrackGenre>()
                .WithOne(tg => tg.Track)
                .HasForeignKey(tg => tg.TrackId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
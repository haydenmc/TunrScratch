using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Tunr.Models;

namespace Tunr.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.2")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRoleClaim<System.Guid>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<Guid>("RoleId");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("RoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserClaim<System.Guid>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<Guid>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("UserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserLogin<System.Guid>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<Guid>("UserId");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("UserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserRole<System.Guid>", b =>
                {
                    b.Property<Guid>("UserId");

                    b.Property<Guid>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("UserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserToken<System.Guid>", b =>
                {
                    b.Property<Guid>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("UserTokens");
                });

            modelBuilder.Entity("Tunr.Models.Library.Track", b =>
                {
                    b.Property<Guid>("TrackId")
                        .ValueGeneratedOnAdd();

                    b.Property<short>("AudioBitrateKbps");

                    b.Property<byte>("AudioChannels");

                    b.Property<short>("AudioDurationSeconds");

                    b.Property<int>("AudioSampleRateHz");

                    b.Property<string>("FileExtension");

                    b.Property<string>("FileName");

                    b.Property<string>("FileRelativePath");

                    b.Property<byte[]>("FileSha256Hash");

                    b.Property<int>("FileSizeBytes");

                    b.Property<long>("LibraryDateTimeAdded");

                    b.Property<long>("LibraryDateTimeModified");

                    b.Property<int>("LibraryPlays");

                    b.Property<byte>("LibraryRating");

                    b.Property<long>("StorageLocation");

                    b.Property<string>("TagAlbum");

                    b.Property<string>("TagAlbumArtist");

                    b.Property<short>("TagAlbumDiscCount");

                    b.Property<short>("TagAlbumTrackCount");

                    b.Property<short>("TagBeatsPerMinute");

                    b.Property<string>("TagComment");

                    b.Property<short>("TagDiscNumber");

                    b.Property<string>("TagTitle");

                    b.Property<short>("TagTrackNumber");

                    b.Property<short>("TagYear");

                    b.Property<Guid>("UserId");

                    b.HasKey("TrackId")
                        .HasAnnotation("SqlServer:Clustered", false);

                    b.HasIndex("UserId");

                    b.ToTable("Tracks");
                });

            modelBuilder.Entity("Tunr.Models.Library.TrackComposer", b =>
                {
                    b.Property<Guid>("TrackId");

                    b.Property<string>("Composer");

                    b.HasKey("TrackId", "Composer");

                    b.ToTable("TrackComposers");
                });

            modelBuilder.Entity("Tunr.Models.Library.TrackGenre", b =>
                {
                    b.Property<Guid>("TrackId");

                    b.Property<string>("Genre");

                    b.HasKey("TrackId", "Genre");

                    b.ToTable("TrackGenres");
                });

            modelBuilder.Entity("Tunr.Models.Library.TrackPerformer", b =>
                {
                    b.Property<Guid>("TrackId");

                    b.Property<string>("Performer");

                    b.HasKey("TrackId", "Performer");

                    b.ToTable("TrackPerformers");
                });

            modelBuilder.Entity("Tunr.Models.TunrRole", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("Tunr.Models.TunrUser", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Email")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<long>("LibraryOffset");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasMaxLength(256);

                    b.HasKey("Id")
                        .HasAnnotation("SqlServer:Clustered", false);

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRoleClaim<System.Guid>", b =>
                {
                    b.HasOne("Tunr.Models.TunrRole")
                        .WithMany("Claims")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserClaim<System.Guid>", b =>
                {
                    b.HasOne("Tunr.Models.TunrUser")
                        .WithMany("Claims")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserLogin<System.Guid>", b =>
                {
                    b.HasOne("Tunr.Models.TunrUser")
                        .WithMany("Logins")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserRole<System.Guid>", b =>
                {
                    b.HasOne("Tunr.Models.TunrRole")
                        .WithMany("Users")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Tunr.Models.TunrUser")
                        .WithMany("Roles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Tunr.Models.Library.Track", b =>
                {
                    b.HasOne("Tunr.Models.TunrUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Tunr.Models.Library.TrackComposer", b =>
                {
                    b.HasOne("Tunr.Models.Library.Track", "Track")
                        .WithMany("DbTagComposers")
                        .HasForeignKey("TrackId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Tunr.Models.Library.TrackGenre", b =>
                {
                    b.HasOne("Tunr.Models.Library.Track", "Track")
                        .WithMany("DbTagGenres")
                        .HasForeignKey("TrackId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Tunr.Models.Library.TrackPerformer", b =>
                {
                    b.HasOne("Tunr.Models.Library.Track", "Track")
                        .WithMany("DbTagPerformers")
                        .HasForeignKey("TrackId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}

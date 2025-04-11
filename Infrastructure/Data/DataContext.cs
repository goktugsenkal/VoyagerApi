using Core.Entities;
using Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
{
    public DbSet<VoyagerUser> Users { get; set; }
    public DbSet<Voyage> Voyages { get; set; }
    public DbSet<Stop> Stops { get; set; }
    public DbSet<Like> Likes { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<UserChangeLog> UserChangeLogs { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseIdentityColumns();

        modelBuilder.Entity<Voyage>()
            .HasOne<VoyagerUser>(v => v.User)
            .WithMany(u => u.Voyages)
            .HasForeignKey(v => v.VoyagerUserId)
            .OnDelete(DeleteBehavior.NoAction);

        var voyagerUserEntity = modelBuilder.Entity<VoyagerUser>();
        
        voyagerUserEntity.HasIndex(u => u.Username)
            .IsUnique();

        voyagerUserEntity.HasMany<UserChangeLog>()
            .WithOne()
            .HasForeignKey(l => l.VoyagerUserId)
            .OnDelete(DeleteBehavior.NoAction);

        var stopEntity = modelBuilder.Entity<Stop>();

        stopEntity.HasOne(s => s.Voyage)
            .WithMany(v => v.Stops)
            .HasForeignKey(s => s.VoyageId)
            .OnDelete(DeleteBehavior.Cascade);

        stopEntity.HasIndex(s => new { s.IsFocalPoint, s.Latitude, s.Longitude });

        modelBuilder.Entity<Like>(entity =>
        {
            // table name
            entity.ToTable("Likes");

            // pk
            entity.HasKey(l => l.Id);

            // relationships
            entity.HasOne(l => l.Voyage)
                .WithMany(v => v.Likes)
                .HasForeignKey(l => l.VoyageId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(l => l.Comment)
                .WithMany(c => c.Likes)
                .HasForeignKey(l => l.CommentId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(l => l.VoyagerUser)
                .WithMany()
                .HasForeignKey(l => l.VoyagerUserId)
                .OnDelete(DeleteBehavior.Cascade);

            // index:
            // composite index for VoyageId, CommentId, and VoyagerUserId ensures uniqueness
            entity.HasIndex(l => new { l.VoyageId, l.CommentId, l.VoyagerUserId })
                .IsUnique()
                .HasDatabaseName("IX_Likes_UniqueUserLike");

            // enum conversion
            entity.Property(l => l.LikeType)
                .IsRequired()
                .HasConversion<int>(); // Enum stored as integer
        });

        
        modelBuilder.Entity<Comment>(entity =>
        {
            // table name
            entity.ToTable("Comments");

            // primary key
            entity.HasKey(c => c.Id);

            // relationships
            entity.HasOne(c => c.Voyage)
                .WithMany(v => v.Comments)
                .HasForeignKey(c => c.VoyageId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(c => c.VoyagerUser)
                .WithMany()
                .HasForeignKey(c => c.VoyagerUserId)
                .OnDelete(DeleteBehavior.Cascade);

            // navigation for likes
            entity.HasMany(c => c.Likes)
                .WithOne(l => l.Comment)
                .HasForeignKey(l => l.CommentId)
                .OnDelete(DeleteBehavior.Cascade);

            // general config
            entity.Property(c => c.Content)
                .IsRequired()
                .HasMaxLength(500); // Optional: Set a max length for comment content
        });

        
        base.OnModelCreating(modelBuilder);
    }
}
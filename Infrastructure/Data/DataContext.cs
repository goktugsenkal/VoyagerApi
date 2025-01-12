using Core.Entities;
using Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
{
    public DbSet<VoyagerUser> Users { get; set; }
    public DbSet<Voyage> Voyages { get; set; }
    public DbSet<Stop> Stops { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseIdentityColumns();

        modelBuilder.Entity<Voyage>()
            .HasOne<VoyagerUser>(v => v.User)
            .WithMany(u => u.Voyages)
            .HasForeignKey(v => v.VoyagerUserId);

        modelBuilder.Entity<Stop>()
            .HasOne<Voyage>(s => s.Voyage)
            .WithMany(v => v.Stops)
            .HasForeignKey(s => s.VoyageId);

        modelBuilder.Entity<Like>()
            .HasOne<Voyage>(l => l.Voyage)
            .WithMany(v => v.Likes)
            .HasForeignKey(l => l.VoyageId);
        
        modelBuilder.Entity<Like>()
            .HasOne<VoyagerUser>(l => l.VoyagerUser)
            .WithMany()
            .HasForeignKey(l => l.VoyagerUserId);
        
        // (VoyageId, VoyagerUserId) pair is unique because 1 person can like something once
        modelBuilder.Entity<Like>()
            .HasIndex(l => new { l.VoyageId, l.VoyagerUserId })
            .IsUnique();
        
        modelBuilder.Entity<Comment>()
            .HasOne<Voyage>(c => c.Voyage)
            .WithMany(c => c.Comments)
            .HasForeignKey(c => c.VoyageId);
        
        modelBuilder.Entity<Comment>()
            .HasOne<VoyagerUser>(c => c.VoyagerUser)
            .WithMany()
            .HasForeignKey(c => c.VoyagerUserId);
        
        base.OnModelCreating(modelBuilder);
    }
}
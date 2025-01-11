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
            
        base.OnModelCreating(modelBuilder);
    }
}
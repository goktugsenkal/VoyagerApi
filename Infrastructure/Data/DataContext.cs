using Core.Entities;
using Core.Entities.Chat;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
{
    public DbSet<VoyagerUser> Users { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<Voyage> Voyages { get; set; }
    public DbSet<Stop> Stops { get; set; }
    public DbSet<Like> Likes { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<UserChangeLog> UserChangeLogs { get; set; }
    
    // chat
    public DbSet<ChatUser> ChatUsers { get; set; }
    public DbSet<ChatRoom> ChatRooms { get; set; }
    public DbSet<ChatRoomParticipant> ChatRoomParticipants { get; set; }
    public DbSet<Message> ChatMessages { get; set; }
    public DbSet<ChatMessageReadReceipt> ChatMessageReadReceipts { get; set; }
    public DbSet<ChatMessageDeliveredReceipt> ChatMessageDeliveredReceipts { get; set; }
    
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
                .HasMaxLength(500);
        });

        // chat table relations
        var chatUserEntity = modelBuilder.Entity<ChatUser>();
        
        chatUserEntity.HasOne(u => u.User)
            .WithOne()
            .HasForeignKey<ChatUser>(u => u.Id)
            .OnDelete(DeleteBehavior.NoAction);
        
        var chatRoomEntity = modelBuilder.Entity<ChatRoom>();

        chatRoomEntity.HasMany(c => c.Participants)
            .WithOne(p => p.ChatRoom)
            .HasForeignKey(p => p.ChatRoomId)
            .OnDelete(DeleteBehavior.Cascade);
        
        chatRoomEntity.HasMany(c => c.Messages)
            .WithOne(m => m.ChatRoom)
            .HasForeignKey(m => m.ChatRoomId)
            .OnDelete(DeleteBehavior.Cascade);
        
        var messageEntity = modelBuilder.Entity<Message>();
        
        messageEntity.HasMany(m => m.MessageReadReceipts)
            .WithOne(d => d.Message)
            .HasForeignKey(d => d.MessageId)
            .OnDelete(DeleteBehavior.Cascade);
        
        messageEntity.HasMany(m => m.MessageDeliveredReceipts)
            .WithOne(r => r.Message)
            .HasForeignKey(r => r.MessageId)
            .OnDelete(DeleteBehavior.Cascade);
        
        messageEntity.HasOne<ChatUser>(m => m.Sender)
            .WithMany()
            .HasForeignKey(m => m.SenderId)
            .OnDelete(DeleteBehavior.NoAction);
        
        // chat indexes
        
        // for filtering by chat room, sorted by date
        messageEntity
            .HasIndex(m => new { m.ChatRoomId, m.CreatedAt })
            .HasDatabaseName("IX_Messages_ChatRoomId_CreatedAt");

        // for checking if user has read a message
        modelBuilder.Entity<ChatMessageReadReceipt>()
            .HasIndex(r => new { r.MessageId, r.UserId })
            .HasDatabaseName("IX_MessageReadReceipts_MessageId_UserId");
        
        // for checking if a message has been delivered
        modelBuilder.Entity<ChatMessageDeliveredReceipt>()
            .HasIndex(r => new { r.MessageId, r.UserId })
            .HasDatabaseName("IX_MessageDeliveredReceipts_MessageId_UserId");
        
        // for getting the rooms a user is in 
        modelBuilder.Entity<ChatRoomParticipant>()
            .HasIndex(p => p.UserId)
            .HasDatabaseName("IX_ChatRoomParticipants_UserId");
        
        modelBuilder.Entity<ChatRoomParticipant>()
            .HasIndex(p => new { p.ChatRoomId, p.UserId })
            .IsUnique()
            .HasDatabaseName("IX_ChatRoomParticipants_ChatRoomId_UserId_Unique");
        
        
        base.OnModelCreating(modelBuilder);
    }
}
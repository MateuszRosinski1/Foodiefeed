using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Foodiefeed_api.entities
{
    public class dbContext : DbContext
    {
        private string _connectionString = "Server=DESKTOP-LN6DFJ3;Database=FoodiefeedDb;Trusted_Connection=True;Encrypt=False";
        public DbSet<User> Users { get; set; }
        public DbSet<Friend> Friends { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<PostCommentMember> PostCommentMembers { get; set; }
        public DbSet<PostImage> PostImages { get; set; }
        public DbSet<PostProduct> PostProducts { get; set; }
        public DbSet<PostTag> PostTags { get; set; }
        public DbSet<UserTag> UserTags { get; set; }
        public DbSet<FriendRequest> FriendRequests { get; set; }
        public virtual DbSet<Follower> Followers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Friend>(entity =>
            {
                entity.HasKey(pc => new { pc.UserId, pc.FriendUserId });
            });

            modelBuilder.Entity<Friend>()
                .HasKey(f => new { f.UserId, f.FriendUserId });

            modelBuilder.Entity<Friend>()
                .HasOne(f => f.User)
                .WithMany(u => u.Friends)
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Friend>()
                .HasOne(f => f.FriendUser)
                .WithMany()
                .HasForeignKey(f => f.FriendUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PostCommentMember>()
                .HasNoKey();


            modelBuilder.Entity<PostCommentMember>()
                .HasKey(pcm => new { pcm.PostId, pcm.CommentId });

            modelBuilder.Entity<PostCommentMember>()
                .HasOne(pcm => pcm.Post)
                .WithMany(p => p.PostCommentMembers)
                .HasForeignKey(pcm => pcm.PostId);

            modelBuilder.Entity<PostCommentMember>()
                .HasOne(pcm => pcm.Comment)
                .WithMany()
                .HasForeignKey(pcm => pcm.CommentId);

            modelBuilder.Entity<Post>()
                .HasMany(p => p.PostProducts)
                .WithOne()
                .HasForeignKey(pp => pp.PostId);

            modelBuilder.Entity<PostCommentMember>()
                .HasOne(pcm => pcm.Post)
                .WithMany(p => p.PostCommentMembers)
                .HasForeignKey(pcm => pcm.PostId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<FriendRequest>()
                .HasKey(fr => new { fr.SenderId, fr.ReceiverId });

            modelBuilder.Entity<FriendRequest>()
                .HasOne(fr => fr.Sender)
                .WithMany(u => u.SendFriendRequests)
                .HasForeignKey(fr => fr.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<FriendRequest>()
                .HasOne(fr => fr.Receiver)
                .WithMany(u => u.ReceivedFriendRequests)
                .HasForeignKey(fr => fr.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Follower>()
                .HasKey(f => new { f.UserId, f.FollowedUserId });

            modelBuilder.Entity<Follower>()
                .HasOne(f => f.User)
                .WithMany(u => u.Followers) 
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Follower>()
                .HasOne(f => f.FollowedUser)
                .WithMany() 
                .HasForeignKey(f => f.FollowedUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString);
        }
    }
}
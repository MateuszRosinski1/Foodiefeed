using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Foodiefeed_api.entities
{
    public class dbContext : DbContext
    {
        private readonly IConfiguration _configuration;

        public dbContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Friend> Friends { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<PostCommentMember> PostCommentMembers { get; set; }
        public DbSet<PostProduct> PostProducts { get; set; }
        public DbSet<PostTag> PostTags { get; set; }
        public DbSet<UserTag> UserTags { get; set; }
        public DbSet<FriendRequest> FriendRequests { get; set; }
        public virtual DbSet<Follower> Followers { get; set; }
        public virtual DbSet<Notification> Notifications { get; set; }
        public virtual DbSet<Tag> Tags { get; set; }
        public virtual DbSet<PostLike> PostLikes { get; set; }
        public virtual DbSet<CommentLike> CommentLikes { get; set; }
        public virtual DbSet<Recipe> Recipes { get; set; }
        public virtual DbSet<Products> Products { get; set; }

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

            modelBuilder.Entity<Notification>()
               .HasOne(n => n.Sender)
               .WithMany()
               .HasForeignKey(n => n.SenderId)
               .OnDelete(DeleteBehavior.NoAction);  

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.Receiver)
                .WithMany()
                .HasForeignKey(n => n.ReceiverId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<PostTag>()
                .HasKey(pt => new { pt.PostId, pt.TagId });

            modelBuilder.Entity<PostTag>()
                .HasOne(pt => pt.Post) 
                .WithMany(p => p.PostTags)
                .HasForeignKey(pt => pt.PostId); 

            modelBuilder.Entity<PostTag>()
                .HasOne(pt => pt.Tag) 
                .WithMany()
                .HasForeignKey(pt => pt.TagId);

            modelBuilder.Entity<UserTag>()
                .HasKey(ut => new { ut.UserId, ut.TagId });

            modelBuilder.Entity<UserTag>()
                .HasOne(ut => ut.User) 
                .WithMany(u => u.UserTags)
                .HasForeignKey(ut => ut.UserId);

            modelBuilder.Entity<UserTag>()
                .HasOne(ut => ut.Tag) 
                .WithMany()
                .HasForeignKey(ut => ut.TagId);

            modelBuilder.Entity<CommentLike>()
        .HasKey(cl => new { cl.CommentId, cl.UserId });

            modelBuilder.Entity<CommentLike>()
                .HasOne(cl => cl.Comment)
                .WithMany(c => c.CommentLikes)
                .HasForeignKey(cl => cl.CommentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CommentLike>()
                .HasOne(cl => cl.User)
                .WithMany(u => u.CommentLikes)
                .HasForeignKey(cl => cl.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PostLike>()
                .HasKey(pl => new { pl.PostId, pl.UserId });

            modelBuilder.Entity<PostLike>()
                .HasOne(pl => pl.Post)
                .WithMany(p => p.PostLikes)
                .HasForeignKey(pl => pl.PostId)
                .OnDelete(DeleteBehavior.Cascade); 

            modelBuilder.Entity<PostLike>()
                .HasOne(pl => pl.User)
                .WithMany(u => u.PostLikes)
                .HasForeignKey(pl => pl.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Recipe>()
                .HasOne(r => r.User)
                .WithMany(u => u.Recipes)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var conStr = _configuration["AZURE_DATABASE_CONSTR"];
            optionsBuilder.UseSqlServer(conStr,opt => opt.CommandTimeout(180));
            optionsBuilder.EnableSensitiveDataLogging();
        }
    }
}
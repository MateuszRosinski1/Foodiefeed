using Bogus;
using Bogus.DataSets;
using Foodiefeed_api.entities;
using System;

namespace Foodiefeed_api
{
    public class DatabaseSeeder
    {
        public static void SeedData(dbContext context)
        {
            if (!context.Users.Any())
            {
                var userFaker = new Faker<User>()
                    .RuleFor(u => u.FirstName, f => f.Name.FirstName())
                    .RuleFor(u => u.LastName, f => f.Name.LastName())
                    .RuleFor(u => u.Username, (f, u) => $"{u.FirstName}.{u.LastName}".ToLower())
                    .RuleFor(u => u.Email, (f, u) => f.Internet.Email(u.FirstName, u.LastName))
                    .RuleFor(u => u.PasswordHash, f => f.Internet.Password())
                    .RuleFor(u => u.ProfilePicturePath, (f, u) => $"images/profilePicture/{u.Id}/default.png")
                    .RuleFor(u => u.IsOnline, f => f.Random.Bool());

                var users = userFaker.Generate(100);

                context.Users.AddRange(users);

                context.SaveChanges();
            }

            if (!context.Posts.Any())
            {
                var postFaker = new Faker<Post>()
                    .RuleFor(p => p.UserId, f => f.PickRandom(context.Users.ToList()).Id)
                    .RuleFor(p => p.Description, f => f.Lorem.Sentence(f.Random.Int(5, 20)))
                    .RuleFor(p => p.Likes, f => f.Random.Int(0, 1000));

                var posts = postFaker.Generate(2000);

                context.Posts.AddRange(posts);
                context.SaveChanges();
            }

            if (!context.Comments.Any())
            {
                var commentFaker = new Faker<Comment>()
                    .RuleFor(c => c.UserId, f => f.PickRandom(context.Users.ToList()).Id)
                    .RuleFor(c => c.CommentContent, f => f.Lorem.Sentence(f.Random.Int(10, 20)))
                    .RuleFor(c => c.Likes, f => f.Random.Int(0, 50));

                var comments = commentFaker.Generate(20000);

                context.Comments.AddRange(comments);
                context.SaveChanges();
            }

            if (!context.PostCommentMembers.Any())
            {
                var comments = context.Comments.ToList();
                var posts = context.Posts.ToList();

                if (comments.Count > 0 && posts.Count > 0)
                {
                    var postCommentMembers = new List<PostCommentMember>();
                    var faker = new Faker(); 

                    foreach (var comment in comments)
                    {
                        var randomPostId = posts[faker.Random.Int(0, posts.Count - 1)].PostId;

                        var postCommentMember = new PostCommentMember
                        {
                            PostId = randomPostId,
                            CommentId = comment.CommentId
                        };

                        postCommentMembers.Add(postCommentMember);
                    }

                    context.PostCommentMembers.AddRange(postCommentMembers);
                    context.SaveChanges();
                }
            }

            if (!context.PostImages.Any())
            {
                var postImageFaker = new Faker<PostImage>()
                    .RuleFor(pi => pi.PostId, f => f.PickRandom(context.Posts.ToList()).PostId)
                    .RuleFor(pi => pi.ImagePath, f => $"images/posts/{f.Random.Int(1, 1000)}.jpg");

                var postImages = postImageFaker.Generate(8000);
                context.PostImages.AddRange(postImages);
                context.SaveChanges();
            }

            if (!context.PostProducts.Any())
            {
                var postProductFaker = new Faker<PostProduct>()
                    .RuleFor(pp => pp.PostId, f => f.PickRandom(context.Posts.ToList()).PostId)
                    .RuleFor(pp => pp.Product, f => f.Commerce.ProductName());

                var postProducts = postProductFaker.Generate(20000);
                context.PostProducts.AddRange(postProducts);
                context.SaveChanges();

            }

            if (!context.Tags.Any())
            {
                List<Tag> tags = new List<Tag>();
                foreach (var tag in Tags.names)
                {
                    tags.Add(new Tag() { Name = tag });
                }

                context.Tags.AddRange(tags);
                context.SaveChanges();
            }

            //if (!context.PostTags.Any())
            //{
            //    var postTagsFaker = new Faker<PostTag>()
            //        .RuleFor(pt => pt.PostId, f => f.PickRandom(context.Posts.ToList()).PostId)
            //        .RuleFor(pt => pt.TagId, f => f.PickRandom(context.Tags.ToList()).Id);

            //    var postTags = postTagsFaker.Generate(10000); // Generuj 200 posttagów

            //    context.PostTags.AddRange(postTags);
            //    context.SaveChanges();
            //}

            if (!context.PostTags.Any())
            {
                var postTagsFaker = new Faker<PostTag>()
                    .RuleFor(pt => pt.PostId, f => f.PickRandom(context.Posts.ToList()).PostId)
                    .RuleFor(pt => pt.TagId, f => f.PickRandom(context.Tags.ToList()).Id)
                    .RuleFor(pt => pt.Description, f => "");

                var postTagsSet = new HashSet<(int PostId, int TagId)>();
                var postTags = new List<PostTag>();

                while (postTags.Count < 10000)
                {
                    var newPostTag = postTagsFaker.Generate();

                    //postTagsSet.Add returns true if the method is able to add an element, otherwise false
                    //hashset was uset cause of efficiency reasons
                    if (postTagsSet.Add((newPostTag.PostId, newPostTag.TagId)))
                    {
                        postTags.Add(newPostTag);
                    }
                }

                context.PostTags.AddRange(postTags);
                context.SaveChanges();
            }

            //if (!context.UserTags.Any())
            //{
            //    var userTagsFaker = new Faker<UserTag>()
            //        .RuleFor(ut => ut.UserId, f => f.PickRandom(context.Users.ToList()).Id) 
            //        .RuleFor(ut => ut.TagId, f => f.PickRandom(context.Tags.ToList()).Id) 
            //        .RuleFor(ut => ut.Score, f => f.Random.Int(1, 100));

            //    var userTags = userTagsFaker.Generate(1000); 

            //    context.UserTags.AddRange(userTags);
            //    context.SaveChanges();
            //}

            if (!context.UserTags.Any())
            {
                var faker = new Faker();
                var userIds = context.Users.Select(u => u.Id).ToList();
                var tagIds = context.Tags.Select(t => t.Id).ToList();
                var userTags = new List<UserTag>(); 

                while (userTags.Count < 1000) 
                {
                    var userId = userIds[faker.Random.Int(0, userIds.Count - 1)];
                    var tagId = tagIds[faker.Random.Int(0, tagIds.Count - 1)];
                    var score = faker.Random.Int(1, 100);

                    // Sprawdzamy, czy rekord już istnieje
                    if (!userTags.Any(ut => ut.UserId == userId && ut.TagId == tagId) &&
                        !context.UserTags.Any(ut => ut.UserId == userId && ut.TagId == tagId)) 
                    {
                        userTags.Add(new UserTag { UserId = userId, TagId = tagId, Score = score });
                    }
                }

                context.UserTags.AddRange(userTags); 
                context.SaveChanges();
            }

            //if (!context.Friends.Any())
            //{            
            //    var faker = new Faker<Friend>()
            //        .RuleFor(f => f.UserId, f => f.PickRandom(context.Users.ToList()).Id) 
            //        .RuleFor(f => f.FriendUserId, (f, friend) =>
            //        {
            //            int friendUserId;
            //            do
            //            {
            //                friendUserId = f.PickRandom(context.Users.ToList()).Id;
            //            }
            //            while (friendUserId == friend.UserId); 

            //            return friendUserId;
            //        });

            //    int friendsCount = 350;
            //    var friends = new List<Friend>();
            //    while (friends.Count < friendsCount)
            //    {
            //        var newFriend = faker.Generate();

            //        var exists = context.Friends.Any(f =>
            //            (f.UserId == newFriend.UserId && f.FriendUserId == newFriend.FriendUserId) ||
            //            (f.UserId == newFriend.FriendUserId && f.FriendUserId == newFriend.UserId));

            //        if (!exists)
            //        {
            //            friends.Add(newFriend);
            //        }
            //    }
            //    context.Friends.AddRange(friends);
            //    context.SaveChanges();
            //}

            if (!context.Friends.Any())
            {
                var faker = new Faker<Friend>()
                    .RuleFor(f => f.UserId, f => f.PickRandom(context.Users.ToList()).Id)
                    .RuleFor(f => f.FriendUserId, (f, friend) =>
                    {
                        int friendUserId;
                        do
                        {
                            friendUserId = f.PickRandom(context.Users.ToList()).Id;
                        }
                        while (friendUserId == friend.UserId); 

                        return friendUserId;
                    });

                int friendsCount = 350;
                var friends = new List<Friend>();
                HashSet<string> uniqueFriendships = new HashSet<string>(); 

                while (friends.Count < friendsCount)
                {
                    var newFriend = faker.Generate();
                    string key = $"{Math.Min(newFriend.UserId, newFriend.FriendUserId)}|{Math.Max(newFriend.UserId, newFriend.FriendUserId)}";

                    if (!uniqueFriendships.Contains(key))
                    {
                        var exists = context.Friends.Any(f =>
                            (f.UserId == newFriend.UserId && f.FriendUserId == newFriend.FriendUserId) ||
                            (f.UserId == newFriend.FriendUserId && f.FriendUserId == newFriend.UserId));

                        if (!exists)
                        {
                            friends.Add(newFriend);
                            uniqueFriendships.Add(key); 
                        }
                    }
                }

                context.Friends.AddRange(friends);
                context.SaveChanges();
            }

            if (!context.FriendRequests.Any())
            {
                var faker = new Faker<FriendRequest>()
                    .RuleFor(fr => fr.SenderId, f => f.PickRandom(context.Users.ToList()).Id)
                    .RuleFor(fr => fr.ReceiverId, (f, fr) =>
                    {
                        int receiverId;
                        do
                        {
                            receiverId = f.PickRandom(context.Users.ToList()).Id;
                        }
                        while (receiverId == fr.SenderId);

                        return receiverId;
                    });

                int requestCount = 400;
                var friendRequests = new List<FriendRequest>();
                HashSet<string> uniqueRequests = new HashSet<string>();

                while (friendRequests.Count < requestCount)
                {
                    var newRequest = faker.Generate();

                    string requestKey = $"{Math.Min(newRequest.SenderId, newRequest.ReceiverId)}|{Math.Max(newRequest.SenderId, newRequest.ReceiverId)}"; 

                    if (!uniqueRequests.Contains(requestKey))
                    {
                        var requestExists = context.FriendRequests.Any(fr =>
                            (fr.SenderId == newRequest.SenderId && fr.ReceiverId == newRequest.ReceiverId));

                        var friendshipExists = context.Friends.Any(f =>
                            (f.UserId == newRequest.SenderId && f.FriendUserId == newRequest.ReceiverId) ||
                            (f.UserId == newRequest.ReceiverId && f.FriendUserId == newRequest.SenderId));

                        if (!requestExists && !friendshipExists)
                        {
                            friendRequests.Add(newRequest);
                            uniqueRequests.Add(requestKey);
                        }
                    }
                }

                context.FriendRequests.AddRange(friendRequests);
                context.SaveChanges();
            }

            if (!context.Followers.Any())
            {
                var faker = new Faker<Follower>()
                    .RuleFor(f => f.UserId, f => f.PickRandom(context.Users.ToList()).Id)
                    .RuleFor(f => f.FollowedUserId, (f, follower) =>
                    {
                        int followedUserId;
                        do
                        {
                            followedUserId = f.PickRandom(context.Users.ToList()).Id;
                        }
                        while (followedUserId == follower.UserId);

                        return followedUserId;
                    });

                int followersCount = 1000;
                var followers = new List<Follower>();
                HashSet<string> uniqueFollowers = new HashSet<string>();

                while (followers.Count < followersCount)
                {
                    var newFollower = faker.Generate();

                    string followerKey = $"{Math.Min(newFollower.UserId, newFollower.FollowedUserId)}|{Math.Max(newFollower.UserId, newFollower.FollowedUserId)}";

                    if (!uniqueFollowers.Contains(followerKey))
                    {
                        var exists = context.Followers.Any(f =>
                            f.UserId == newFollower.UserId && f.FollowedUserId == newFollower.FollowedUserId);

                        if (!exists)
                        {
                            followers.Add(newFollower);
                            uniqueFollowers.Add(followerKey); 
                        }
                    }
                }

                context.Followers.AddRange(followers);
                context.SaveChanges();
            }

            if (!context.Notifications.Any()) {
                
                List<Notification> notifications = new List<Notification>();
                foreach(var request in context.FriendRequests.ToList())
                {
                    notifications.Add(new Notification(NotificationType.FriendRequest) { SenderId = request.SenderId, ReceiverId = request.ReceiverId });
                }

                foreach(var friend in context.Friends.ToList())
                {
                    notifications.Add(new Notification(NotificationType.AcceptedFriendRequest) { SenderId = friend.UserId, ReceiverId = friend.FriendUserId });
                }

                context.Notifications.AddRange(notifications);
                context.SaveChanges();
            }


            
        }
    }
}

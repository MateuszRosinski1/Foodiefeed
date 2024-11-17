using Azure.Core;
using Bogus;
using Bogus.DataSets;
using Foodiefeed_api.entities;
using Microsoft.EntityFrameworkCore;
using System;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Newtonsoft.Json;

namespace Foodiefeed_api
{
    public class DatabaseSeeder
    {
        //https://www.crcv.ucf.edu/data/Selfie/ dataset for selfies
        //https://www.kaggle.com/datasets/trolukovich/food11-image-dataset dataset for dishes images
        //https://github.com/karansikka1/iFood_2019?tab=readme-ov-file dish images
        //https://www.kaggle.com/datasets/kaggle/recipe-ingredients-dataset?resource=download products dataset
        public static async void SeedData(dbContext context)
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


            //var blobSerivceClient = new BlobServiceClient("");
            //var containerClient = blobSerivceClient.GetBlobContainerClient("images-storage");

            //var selfiesPathLocal = "D:/ImageDataset/Selfie-dataset/Selfie-dataset/images";
            //var random = new Random();
            //var selfies = Directory.GetFiles(selfiesPathLocal, "*.jpg");

            //foreach (var user in context.Users.ToList())
            //{
            //    var dir = $"{user.Id}/";

            //    var profilePicturePath = dir + "pfp.jpg";

            //    var randomProfilePicture = selfies[random.Next(selfies.Length)];

            //    var blobClient = containerClient.GetBlobClient(profilePicturePath);

            //    using (var stream = File.OpenRead(randomProfilePicture))
            //    {
            //        await blobClient.UploadAsync(stream, true);
            //    }
            //}

            //var imageDatasetPath = "D:/ImageDataset/train/train_set/";

            //var imagesPath = Directory.GetFiles(imageDatasetPath, "*.jpg");

            //foreach (var post in context.Posts.ToList())
            //{
            //    var dir = $"{post.UserId}/posts/{post.PostId}/";
            //    var random = new Random();
            //    var imageCount = random.Next(1, 10);

            //    for(int  i =1 ; i <= imageCount; i++)
            //    {
            //        var imgPath = dir + i.ToString() + ".jpg";

            //        var blobClient = containerClient.GetBlobClient(imgPath);

            //        var randomImage = imagesPath[random.Next(imagesPath.Length)];

            //        using (var stream = File.OpenRead(randomImage))
            //        {
            //            await blobClient.UploadAsync(stream,true);
            //        }


            //    }
            //}

            //foreach(var user in context.Users.ToList())
            //{
            //    var userFolder = $"{user.Id}/posts/";

            //    var blobClient = containerClient.GetBlobClient($"{userFolder}placeholder.txt");
            //    
            //    var placeholderContent = new BinaryData("Folder placeholder");
            //    blobClient.Upload(placeholderContent, overwrite: true);
            //}

            if (!context.Posts.Any())
            {
                var postFaker = new Faker<Post>()
                    .RuleFor(p => p.UserId, f => f.PickRandom(context.Users.ToList()).Id)
                    .RuleFor(p => p.Description, f => f.Lorem.Sentence(f.Random.Int(5, 400)));

                var posts = postFaker.Generate(2000);

                context.Posts.AddRange(posts);
                context.SaveChanges();
            }

            if (!context.PostLikes.Any())
            {
                var postLikeFaker = new Faker<PostLike>()
                    .RuleFor(p => p.PostId, f => f.PickRandom(context.Posts.ToList()).PostId)
                    .RuleFor(p => p.UserId, f => f.PickRandom(context.Users.ToList()).Id);

                var existingPostLikes = context.PostLikes.ToList();
                var postLikesToAdd = new List<PostLike>();

                for (int i = 0; i < 10000; i++) 
                {
                    var newPostLike = postLikeFaker.Generate();

                    var exists = postLikesToAdd.FirstOrDefault(p => p.PostId == newPostLike.PostId && p.UserId == newPostLike.UserId);

                    if (exists is null)
                    {
                        postLikesToAdd.Add(newPostLike);
                    }
                }

                context.PostLikes.AddRange(postLikesToAdd);
                context.SaveChanges();
            }

            if (!context.Comments.Any())
            {
                var commentFaker = new Faker<Comment>()
                    .RuleFor(c => c.UserId, f => f.PickRandom(context.Users.ToList()).Id)
                    .RuleFor(c => c.CommentContent, f => f.Lorem.Sentence(f.Random.Int(10, 100)));

                var comments = commentFaker.Generate(20000);

                context.Comments.AddRange(comments);
                context.SaveChanges();
            }

            if (!context.CommentLikes.Any())
            {
                var commentLikeFaker = new Faker<CommentLike>()
                    .RuleFor(c => c.CommentId, f => f.PickRandom(context.Comments.ToList()).CommentId)
                    .RuleFor(c => c.UserId, f => f.PickRandom(context.Users.ToList()).Id);

                var commentLikesToAdd = new List<CommentLike>();
                for (int i = 0; i < 10000; i++)
                {
                    var newCommentLike = commentLikeFaker.Generate();

                    var exists = commentLikesToAdd.FirstOrDefault(cl => cl.CommentId == newCommentLike.CommentId && cl.UserId == newCommentLike.UserId);
                    if (exists is null)
                    {
                        commentLikesToAdd.Add(newCommentLike);
                    }
                }

                context.CommentLikes.AddRange(commentLikesToAdd);
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

            if (!context.Products.Any())
            {
                var products = Product.ProductNames;

                List<Products> productsList = new List<Products>();

                foreach (var product in products)
                {
                    productsList.Add(new Products() { Name = product });
                }

                context.Products.AddRange(productsList);
                context.SaveChanges();
            }

            if (!context.PostProducts.Any())
            {
                var postProductFaker = new Faker<PostProduct>()
                     .RuleFor(pp => pp.PostId, f => f.PickRandom(context.Posts.ToList()).PostId)
                     .RuleFor(pp => pp.ProductId, f => f.PickRandom(context.Products.ToList()).Id);

                var postProducts = postProductFaker.Generate(20000);

                var noDupes = new List<PostProduct>();

                foreach(var product in postProducts)
                {
                    if (!noDupes.Contains(product))
                    {
                        noDupes.Add(product);
                    }
                }

                context.PostProducts.AddRange(noDupes);
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

            if (!context.PostTags.Any())
            {
                var postTags = new List<PostTag>();
                var allTags = context.Tags.ToList();
                var posts = context.Posts.Include(p => p.PostTags).ToList();
                foreach (var post in posts)
                {

                    while (post.PostTags.Count < 4)
                    {
                        var randomTag = allTags[new Random().Next(allTags.Count)];

                        bool tagExists = context.PostTags.Any(pt => pt.PostId == post.PostId && pt.TagId == randomTag.Id);

                        if (!tagExists)
                        {
                            post.PostTags.Add(new PostTag { PostId = post.PostId, TagId = randomTag.Id});
                            context.SaveChanges();
                        }
                    }
                }
            }

            if (!context.UserTags.Any())
            {
                var faker = new Faker();
                var userIds = context.Users.Select(u => u.Id).ToList();
                var tagIds = context.Tags.Select(t => t.Id).ToList();

                foreach (var id in userIds)
                {
                    var user = await context.Users.Include(u => u.UserTags).FirstAsync(u => u.Id == id);

                    foreach(var tag in tagIds)
                    {
                        var initTag = new UserTag()
                        {
                            UserId = user.Id,
                            TagId = tag,
                            Score = faker.Random.Int(0, 100)
                        };
                        user.UserTags.Add(initTag);
                    }
                }

                context.SaveChanges();
            }

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
                    var user = context.Users.FirstOrDefault(u => u.Id == request.SenderId);
                    if(user is not null)
                    {
                        notifications.Add(new Notification(NotificationType.FriendRequest,user.Username) { SenderId = request.SenderId, ReceiverId = request.ReceiverId });
                    }
                }

                foreach(var friend in context.Friends.ToList())
                {
                    var user = context.Users.FirstOrDefault(u => u.Id == friend.UserId);
                    if(user is not null) {
                        notifications.Add(new Notification(NotificationType.AcceptedFriendRequest, user.Username) { SenderId = friend.UserId, ReceiverId = friend.FriendUserId });
                    }
                    var user2 = context.Users.FirstOrDefault(u => u.Id == friend.FriendUserId);
                    if (user2 is not null)
                    {
                        notifications.Add(new Notification(NotificationType.AcceptedFriendRequest, user2.Username) { SenderId = friend.FriendUserId, ReceiverId = friend.UserId });
                    }
                }

                foreach(var comment in context.CommentLikes.ToList()) 
                {
                    var user = context.Users.FirstOrDefault(u => u.Id == comment.UserId);
                    var userComment = context.Comments.FirstOrDefault(c => c.CommentId == comment.CommentId);
                    if (user is not null && userComment is not null)
                    {
                        notifications.Add(new Notification(NotificationType.CommentLike, user.Username) { SenderId = comment.UserId, ReceiverId = userComment.UserId,CommentId = comment.CommentId });
                    }
                }

                foreach(var post in context.PostLikes.ToList())
                {
                    var user = context.Users.FirstOrDefault(u => u.Id ==post.UserId);
                    var userPost = context.Posts.FirstOrDefault(p => p.PostId == post.PostId);
                    if(user is not null && userPost is not null)
                    {
                        notifications.Add(new Notification(NotificationType.PostLike, user.Username) { SenderId = post.UserId, ReceiverId = userPost.UserId, PostId = post.PostId });
                    }
                }

                foreach(var follower in context.Followers.ToList())
                {
                    var receiver = context.Users.FirstOrDefault(u => u.Id == follower.FollowedUserId);
                    var sender = context.Users.FirstOrDefault(u => u.Id == follower.UserId);

                    if(receiver is not null && sender is not null)
                    {
                        notifications.Add(new Notification(NotificationType.GainFollower, sender.Username) { SenderId = sender.Id,ReceiverId = receiver.Id });
                    }
                }

                foreach (var post in context.Posts.Include(p => p.PostCommentMembers).ToList())
                {
                    var reciever = context.Users.FirstOrDefault(u => u.Id ==post.UserId);
                    if( reciever is not null )
                    {
                        foreach (var member in post.PostCommentMembers)
                        {
                            var comment = context.Comments.FirstOrDefault(c => c.CommentId == member.CommentId);
                            if (comment is not null)
                            {
                                var sender = context.Users.FirstOrDefault(u => u.Id == comment.UserId);

                                if (sender is not null)
                                {
                                    notifications.Add(new Notification(NotificationType.PostComment, sender.Username)
                                    {
                                        ReceiverId = reciever.Id,
                                        SenderId = sender.Id,
                                        PostId = post.PostId,
                                        CommentId = comment.CommentId,

                                    });
                                }
                            }
                        }
                    }
                }
                context.Notifications.AddRange(notifications);
                context.SaveChanges();
            }

            if (!context.Recipes.Any())
            {
                var recipeFaker = new Faker<Recipe>()
                    .RuleFor(r => r.UserId, f => f.PickRandom(context.Users.ToList()).Id)
                    .RuleFor(r => r.PostId, f => f.PickRandom(context.Posts.ToList()).PostId);

                var recipes = recipeFaker.Generate(2000);

                List<Recipe> noDupesRecipes = new List<Recipe>();       
                
                foreach(var recipe in recipes)
                {
                    if (!noDupesRecipes.Contains(recipe))
                    {
                        noDupesRecipes.Add(recipe);
                    }
                }
                await context.Recipes.AddRangeAsync(noDupesRecipes);
                context.SaveChanges();
            }
        }
    }
}

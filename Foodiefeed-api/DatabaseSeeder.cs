using Foodiefeed_api.entities;
using System;

namespace Foodiefeed_api
{
    public class DatabaseSeeder
    {
        public static void SeedData(dbContext context)
        {
            public static void SeedData(AppDbContext context)
            {
                // Pobierz użytkowników z bazy danych
                var userMati = context.Users.FirstOrDefault(u => u.Username == "mati");
                var userMariuszek = context.Users.FirstOrDefault(u => u.Username == "Mariuszek29");

                if (userMati == null || userMariuszek == null)
                {
                    throw new Exception("Użytkownicy 'mati' i 'Mariuszek29' muszą istnieć w bazie danych przed uruchomieniem seeder'a.");
                }

                // Dodaj posty dla istniejących użytkowników
                if (!context.Posts.Any())
                {
                    var posts = new List<Post>
            {
                new Post
                {
                    UserId = userMati.Id,
                    Description = "Właśnie wróciłem z wakacji!",
                    Likes = 25
                },
                new Post
                {
                    UserId = userMariuszek.Id,
                    Description = "Dziś miałem świetny dzień na treningu.",
                    Likes = 42
                }
            };
                    context.Posts.AddRange(posts);
                    context.SaveChanges();
                }

                // Dodaj komentarze dla postów
                if (!context.Comments.Any())
                {
                    var comments = new List<Comment>
            {
                new Comment
                {
                    UserId = userMariuszek.Id,
                    CommentContent = "Wygląda na to, że świetnie się bawiłeś!",
                    Likes = 10
                },
                new Comment
                {
                    UserId = userMati.Id,
                    CommentContent = "Trening to podstawa!",
                    Likes = 15
                }
            };
                    context.Comments.AddRange(comments);
                    context.SaveChanges();
                }

                // Dodaj relacje PostCommentMember
                if (!context.PostCommentMembers.Any())
                {
                    var postCommentMembers = new List<PostCommentMember>
            {
                new PostCommentMember
                {
                    PostId = context.Posts.First().PostId,
                    CommentId = context.Comments.First().CommentId
                }
            };
                    context.PostCommentMembers.AddRange(postCommentMembers);
                    context.SaveChanges();
                }

                // Dodaj obrazy do postów
                if (!context.PostImages.Any())
                {
                    var postImages = new List<PostImage>
            {
                new PostImage
                {
                    PostId = context.Posts.First().PostId,
                    ImagePath = "/images/vacation.png"
                }
            };
                    context.PostImages.AddRange(postImages);
                    context.SaveChanges();
                }

                // Dodaj produkty do postów
                if (!context.PostProducts.Any())
                {
                    var postProducts = new List<PostProduct>
            {
                new PostProduct
                {
                    PostId = context.Posts.First().PostId,
                    Product = "Sportowy zegarek"
                }
            };
                    context.PostProducts.AddRange(postProducts);
                    context.SaveChanges();
                }

                // Dodaj tagi do postów
                if (!context.PostTags.Any())
                {
                    var postTags = new List<PostTag>
            {
                new PostTag
                {
                    PostId = context.Posts.First().PostId,
                    TagName = "fitness",
                    Description = "Posty o fitnessie"
                }
            };
                    context.PostTags.AddRange(postTags);
                    context.SaveChanges();
                }

                // Dodaj tagi użytkownika (UserTags)
                if (!context.UserTags.Any())
                {
                    var userTags = new List<UserTag>
            {
                new UserTag
                {
                    UserId = userMati.Id,
                    TagName = "podróże",
                    Count = 100
                },
                new UserTag
                {
                    UserId = userMariuszek.Id,
                    TagName = "fitness",
                    Count = 150
                }
            };
                    context.UserTags.AddRange(userTags);
                    context.SaveChanges();
                }

                // Dodaj znajomych
                if (!context.Friends.Any())
                {
                    var friends = new List<Friend>
            {
                new Friend
                {
                    UserId = userMati.Id,
                    FriendUserId = userMariuszek.Id
                }
            };
                    context.Friends.AddRange(friends);
                    context.SaveChanges();
                }

                // Dodaj zaproszenia do znajomych
                if (!context.FriendRequests.Any())
                {
                    var friendRequests = new List<FriendRequest>
            {
                new FriendRequest
                {
                    SenderId = userMariuszek.Id,
                    ReceiverId = userMati.Id
                }
            };
                    context.FriendRequests.AddRange(friendRequests);
                    context.SaveChanges();
                }
            }
        }
    }
}
